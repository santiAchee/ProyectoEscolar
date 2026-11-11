using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public InscripcionesController(SistemaEscolarContext context)
        {
            _context = context;
        }

        // Inicializa los 4 documentos obligatorios del relevamiento si la tabla está vacía
        private async Task AsegurarTiposDocumentosAsync()
        {
            if (!await _context.TiposDocumentos.AnyAsync())
            {
                var obligatorios = new List<TiposDocumento>
                {
                    new TiposDocumento { Nombre = "Fotocopia de DNI", Obligatorio = true, Activo = true },
                    new TiposDocumento { Nombre = "Libreta de Vacunas", Obligatorio = true, Activo = true },
                    new TiposDocumento { Nombre = "Certificado de Aptitud Física", Obligatorio = true, Activo = true },
                    new TiposDocumento { Nombre = "Pase de Escuela Anterior", Obligatorio = true, Activo = true }
                };
                _context.TiposDocumentos.AddRange(obligatorios);
                await _context.SaveChangesAsync();
            }
        }

        // 1. LISTAR TODAS LAS INSCRIPCIONES CON DATOS DE ALUMNO, CURSO Y DOCUMENTOS
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await AsegurarTiposDocumentosAsync();

            var inscripciones = await _context.Inscripciones
                .Include(i => i.IdAlumnoNavigation)
                    .ThenInclude(a => a.IdTutorNavigation)
                .Include(i => i.IdCursoNavigation)
                    .ThenInclude(c => c.IdDocenteNavigation)
                .Include(i => i.InscripcionDocumentaciones)
                    .ThenInclude(id => id.IdDocumentacionNavigation)
                        .ThenInclude(d => d.IdTipoDocumentoNavigation)
                .Where(i => i.Activo == true)
                .OrderByDescending(i => i.FechaSolicitud)
                .Select(i => new
                {
                    i.IdInscripcion,
                    i.IdAlumno,
                    AlumnoNombre = i.IdAlumnoNavigation.Apellido + ", " + i.IdAlumnoNavigation.Nombre,
                    AlumnoDni = i.IdAlumnoNavigation.Dni,
                    TutorNombre = i.IdAlumnoNavigation.IdTutorNavigation != null
                        ? i.IdAlumnoNavigation.IdTutorNavigation.Apellido + ", " + i.IdAlumnoNavigation.IdTutorNavigation.Nombre
                        : "Sin tutor",
                    i.IdCurso,
                    CursoDescripcion = $"{i.IdCursoNavigation.Nivel} - {i.IdCursoNavigation.AnioCurso}° '{i.IdCursoNavigation.Division}' ({i.IdCursoNavigation.Turno})",
                    DocenteNombre = i.IdCursoNavigation.IdDocenteNavigation != null
                        ? i.IdCursoNavigation.IdDocenteNavigation.Apellido + ", " + i.IdCursoNavigation.IdDocenteNavigation.Nombre
                        : "Sin docente",
                    i.CicloLectivo,
                    i.FechaSolicitud,
                    i.EstadoInscripcion,
                    i.FechaResolucion,
                    i.Observacion,
                    Documentos = i.InscripcionDocumentaciones.Select(d => new
                    {
                        d.IdDocumentacion,
                        NombreDocumento = d.IdDocumentacionNavigation.IdTipoDocumentoNavigation.Nombre,
                        Obligatorio = d.IdDocumentacionNavigation.IdTipoDocumentoNavigation.Obligatorio,
                        d.Presentado,
                        d.Validado
                    }).ToList()
                })
                .ToListAsync();

            return Ok(inscripciones);
        }

        // 2. REPORTE DIARIO DE VACANTES Y CUPOS POR CURSO (Requerimiento de Secretaría)
        [HttpGet("vacantes")]
        public async Task<IActionResult> GetReporteVacantes([FromQuery] int cicloLectivo = 2026)
        {
            var cursos = await _context.Cursos
                .Include(c => c.IdDocenteNavigation)
                .Include(c => c.Inscripciones)
                .Where(c => c.Activo == true)
                .Select(c => new
                {
                    c.IdCurso,
                    Curso = $"{c.Nivel} - {c.AnioCurso}° '{c.Division}' ({c.Turno})",
                    Docente = c.IdDocenteNavigation != null ? c.IdDocenteNavigation.Apellido + ", " + c.IdDocenteNavigation.Nombre : "-",
                    c.CupoMaximo,
                    InscriptosConfirmados = c.Inscripciones.Count(i => i.Activo == true && i.CicloLectivo == cicloLectivo && i.EstadoInscripcion == "Inscripto"),
                    PendientesReserva = c.Inscripciones.Count(i => i.Activo == true && i.CicloLectivo == cicloLectivo && i.EstadoInscripcion == "Pendiente"),
                    EnListaEspera = c.Inscripciones.Count(i => i.Activo == true && i.CicloLectivo == cicloLectivo && i.EstadoInscripcion == "En Lista de Espera"),
                    VacantesDisponibles = c.CupoMaximo - c.Inscripciones.Count(i => i.Activo == true && i.CicloLectivo == cicloLectivo && (i.EstadoInscripcion == "Inscripto" || i.EstadoInscripcion == "Pendiente"))
                })
                .ToListAsync();

            return Ok(cursos);
        }

        // 3. REGISTRAR SOLICITUD DE INSCRIPCIÓN (Asigna Alumno a Curso y verifica Cupo)
        public class NuevaInscripcionRequest
        {
            public int IdAlumno { get; set; }
            public int IdCurso { get; set; }
            public int CicloLectivo { get; set; } = 2026;
            public string? Observacion { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NuevaInscripcionRequest req)
        {
            await AsegurarTiposDocumentosAsync();

            var alumno = await _context.Alumnos.FindAsync(req.IdAlumno);
            if (alumno == null || alumno.Activo != true)
                return BadRequest(new { mensaje = "El alumno seleccionado no existe o está inactivo." });

            var curso = await _context.Cursos.FindAsync(req.IdCurso);
            if (curso == null || curso.Activo != true)
                return BadRequest(new { mensaje = "El curso seleccionado no existe o está inactivo." });

            // Evitar duplicar solicitud activa en el mismo ciclo lectivo
            var yaTieneSolicitud = await _context.Inscripciones.AnyAsync(i =>
                i.IdAlumno == req.IdAlumno &&
                i.CicloLectivo == req.CicloLectivo &&
                i.Activo == true &&
                i.EstadoInscripcion != "Baja/Rechazado");

            if (yaTieneSolicitud)
                return BadRequest(new { mensaje = "El alumno ya posee una inscripción activa o en proceso para este ciclo lectivo." });

            // Contar lugares ocupados provisoria o definitivamente ("Pendiente" + "Inscripto")
            var lugaresOcupados = await _context.Inscripciones.CountAsync(i =>
                i.IdCurso == req.IdCurso &&
                i.CicloLectivo == req.CicloLectivo &&
                i.Activo == true &&
                (i.EstadoInscripcion == "Inscripto" || i.EstadoInscripcion == "Pendiente"));

            // Según Diagrama de Actividades: ¿Hay vacantes? Sí -> Pendiente (para entregar docs) | No -> En Lista de Espera
            string estadoInicial = (lugaresOcupados < curso.CupoMaximo) ? "Pendiente" : "En Lista de Espera";

            var nuevaInscripcion = new Inscripcione
            {
                IdAlumno = req.IdAlumno,
                IdCurso = req.IdCurso,
                CicloLectivo = req.CicloLectivo,
                FechaSolicitud = DateTime.Now,
                EstadoInscripcion = estadoInicial,
                Observacion = req.Observacion,
                Activo = true
            };

            _context.Inscripciones.Add(nuevaInscripcion);
            await _context.SaveChangesAsync();

            // Crear los registros de documentación requeridos para este alumno
            var tiposDoc = await _context.TiposDocumentos.Where(t => t.Activo == true).ToListAsync();
            foreach (var tipo in tiposDoc)
            {
                var doc = new Documentacione
                {
                    IdTipoDocumento = tipo.IdTipoDocumento,
                    FechaEmision = DateOnly.FromDateTime(DateTime.Now)
                };
                _context.Documentaciones.Add(doc);
                await _context.SaveChangesAsync();

                var inscDoc = new InscripcionDocumentacione
                {
                    IdInscripcion = nuevaInscripcion.IdInscripcion,
                    IdDocumentacion = doc.IdDocumentacion,
                    Presentado = false,
                    Validado = false
                };
                _context.InscripcionDocumentaciones.Add(inscDoc);
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = estadoInicial == "Pendiente"
                    ? "Solicitud registrada en estado 'Pendiente' (vacante reservada provisoriamente). Valide la documentación para confirmar la inscripción."
                    : "El curso no tiene cupos disponibles. El alumno fue ubicado automáticamente 'En Lista de Espera'.",
                estadoAsignado = estadoInicial,
                idInscripcion = nuevaInscripcion.IdInscripcion
            });
        }

        // 4. ACTUALIZAR / VALIDAR DOCUMENTACIÓN Y AUTO-CONFIRMAR INSCRIPCIÓN
        public class ActualizarDocRequest
        {
            public int IdDocumentacion { get; set; }
            public bool Presentado { get; set; }
            public bool Validado { get; set; }
        }

        [HttpPut("{idInscripcion}/documentacion")]
        public async Task<IActionResult> ActualizarDocumentacion(int idInscripcion, [FromBody] List<ActualizarDocRequest> docs)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.IdCursoNavigation)
                .Include(i => i.InscripcionDocumentaciones)
                    .ThenInclude(id => id.IdDocumentacionNavigation)
                        .ThenInclude(d => d.IdTipoDocumentoNavigation)
                .FirstOrDefaultAsync(i => i.IdInscripcion == idInscripcion && i.Activo == true);

            if (inscripcion == null) return NotFound(new { mensaje = "Inscripción no encontrada." });

            foreach (var item in docs)
            {
                var reg = inscripcion.InscripcionDocumentaciones.FirstOrDefault(d => d.IdDocumentacion == item.IdDocumentacion);
                if (reg != null)
                {
                    reg.Presentado = item.Presentado;
                    reg.FechaPresentacion = item.Presentado ? DateTime.Now : null;
                    reg.Validado = item.Validado;
                    reg.FechaValidacion = item.Validado ? DateTime.Now : null;
                }
            }

            // Verificar si todos los documentos obligatorios están presentados y validados
            bool todosValidados = inscripcion.InscripcionDocumentaciones
                .Where(d => d.IdDocumentacionNavigation.IdTipoDocumentoNavigation.Obligatorio == true)
                .All(d => d.Presentado == true && d.Validado == true);

            if (todosValidados && inscripcion.EstadoInscripcion != "Inscripto")
            {
                // Verificar cupo si venía de Lista de Espera o Baja
                var ocupadosSinEste = await _context.Inscripciones.CountAsync(i =>
                    i.IdCurso == inscripcion.IdCurso &&
                    i.CicloLectivo == inscripcion.CicloLectivo &&
                    i.IdInscripcion != idInscripcion &&
                    i.Activo == true &&
                    (i.EstadoInscripcion == "Inscripto" || i.EstadoInscripcion == "Pendiente"));

                if (ocupadosSinEste < inscripcion.IdCursoNavigation.CupoMaximo)
                {
                    inscripcion.EstadoInscripcion = "Inscripto";
                    inscripcion.FechaResolucion = DateTime.Now;
                }
                else
                {
                    await _context.SaveChangesAsync();
                    return BadRequest(new { mensaje = "Documentación guardada, pero no se pudo pasar a 'Inscripto' porque el curso alcanzó su cupo máximo." });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Documentación actualizada correctamente.", nuevoEstado = inscripcion.EstadoInscripcion });
        }

        // 5. CAMBIAR ESTADO MANUALMENTE (Habilitar desde Lista de Espera, Pasar a Pendiente o Rechazar/Dar de Baja)
        public class CambiarEstadoRequest
        {
            public string NuevoEstado { get; set; } = null!; // 'Pendiente', 'Inscripto', 'En Lista de Espera', 'Baja/Rechazado'
            public string? Observacion { get; set; }
        }

        [HttpPut("{idInscripcion}/estado")]
        public async Task<IActionResult> CambiarEstado(int idInscripcion, [FromBody] CambiarEstadoRequest req)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.IdCursoNavigation)
                .Include(i => i.InscripcionDocumentaciones)
                    .ThenInclude(id => id.IdDocumentacionNavigation)
                        .ThenInclude(d => d.IdTipoDocumentoNavigation)
                .FirstOrDefaultAsync(i => i.IdInscripcion == idInscripcion && i.Activo == true);

            if (inscripcion == null) return NotFound(new { mensaje = "Inscripción no encontrada." });

            // Si quiere pasar a Inscripto o Pendiente desde Lista de Espera o Baja, validar que haya cupo disponible
            if (req.NuevoEstado == "Inscripto" || req.NuevoEstado == "Pendiente")
            {
                var ocupados = await _context.Inscripciones.CountAsync(i =>
                    i.IdCurso == inscripcion.IdCurso &&
                    i.CicloLectivo == inscripcion.CicloLectivo &&
                    i.IdInscripcion != idInscripcion &&
                    i.Activo == true &&
                    (i.EstadoInscripcion == "Inscripto" || i.EstadoInscripcion == "Pendiente"));

                if (ocupados >= inscripcion.IdCursoNavigation.CupoMaximo)
                    return BadRequest(new { mensaje = $"No hay cupo disponible en el curso (Máximo: {inscripcion.IdCursoNavigation.CupoMaximo}). Debe liberar una vacante primero." });
            }

            // Si pasa a "Inscripto" directamente, marcamos sus documentos como validados
            if (req.NuevoEstado == "Inscripto")
            {
                foreach (var doc in inscripcion.InscripcionDocumentaciones)
                {
                    doc.Presentado = true;
                    doc.Validado = true;
                    doc.FechaPresentacion ??= DateTime.Now;
                    doc.FechaValidacion ??= DateTime.Now;
                }
            }

            inscripcion.EstadoInscripcion = req.NuevoEstado;
            if (!string.IsNullOrWhiteSpace(req.Observacion))
                inscripcion.Observacion = req.Observacion;

            if (req.NuevoEstado == "Inscripto" || req.NuevoEstado == "Baja/Rechazado")
                inscripcion.FechaResolucion = DateTime.Now;

            await _context.SaveChangesAsync();

            // Si se liberó un lugar por "Baja/Rechazado", avisar o promover si corresponde
            return Ok(new { mensaje = $"Estado actualizado a '{req.NuevoEstado}'." });
        }
    }
}