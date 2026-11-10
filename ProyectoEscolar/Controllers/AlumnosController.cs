using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bloquea el acceso a quien no tenga un JWT válido
    public class AlumnosController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public AlumnosController(SistemaEscolarContext context)
        {
            _context = context;
        }

        // GET: api/Alumnos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var alumnos = await _context.Alumnos
                .Where(a => a.Activo == true)
                .ToListAsync();

            return Ok(alumnos);
        }

        // POST: api/Alumnos
        [HttpPost]
        public async Task<IActionResult> Post(Alumno alumno)
        {
            // 1. Validación de edad mínima (al menos 3 años)
            if ((DateTime.Now.Year - alumno.FechaNacimiento.Year) < 3)
            {
                return BadRequest(new { mensaje = "El alumno debe tener al menos 3 años." });
            }

            // 2. Validación de DNI duplicado
            var dniExiste = await _context.Alumnos.AnyAsync(a => a.Dni == alumno.Dni);
            if (dniExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un alumno registrado con ese DNI." });
            }

            // 3. Validación de existencia del Tutor (por clave foránea)
            var tutorExiste = await _context.Tutores.AnyAsync(t => t.IdTutor == alumno.IdTutor);
            if (!tutorExiste)
            {
                return BadRequest(new { mensaje = "El ID de tutor especificado no existe en la base de datos." });
            }

            // Configuración por defecto
            alumno.Activo = true;

            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();

            return Ok(alumno);
        }

        // PUT: api/Alumnos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Alumno alumno)
        {
            if (id != alumno.IdAlumno)
            {
                return BadRequest(new { mensaje = "El ID no coincide." });
            }

            // Validar existencia del tutor antes de actualizar
            var tutorExiste = await _context.Tutores.AnyAsync(t => t.IdTutor == alumno.IdTutor);
            if (!tutorExiste)
            {
                return BadRequest(new { mensaje = "El ID de tutor especificado no existe." });
            }

            _context.Alumnos.Update(alumno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Alumnos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }

            alumno.Activo = false; // Baja lógica para no romper relaciones con inscripciones
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}