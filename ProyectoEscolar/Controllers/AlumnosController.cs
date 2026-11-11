using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public AlumnosController(SistemaEscolarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var alumnos = await _context.Alumnos
                .Where(a => a.Activo == true)
                .ToListAsync();
            return Ok(alumnos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Alumno alumno)
        {
            if (await _context.Alumnos.AnyAsync(a => a.Dni == alumno.Dni))
                return BadRequest(new { mensaje = "Ya existe un alumno con ese DNI." });

            if (alumno.IdTutor.HasValue)
            {
                var tutorExiste = await _context.Tutores.AnyAsync(t => t.IdTutor == alumno.IdTutor.Value && t.Activo == true);
                if (!tutorExiste)
                    return BadRequest(new { mensaje = "El tutor indicado no existe o está dado de baja." });
            }

            alumno.IdAlumno = 0;
            alumno.Activo = true;
            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();

            return Ok(alumno);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Alumno alumno)
        {
            var existente = await _context.Alumnos.FindAsync(id);
            if (existente == null) return NotFound(new { mensaje = "Alumno no encontrado." });

            existente.Nombre = alumno.Nombre;
            existente.Apellido = alumno.Apellido;
            existente.Dni = alumno.Dni;
            existente.FechaNacimiento = alumno.FechaNacimiento;
            existente.Direccion = alumno.Direccion;
            existente.Telefono = alumno.Telefono;
            existente.IdTutor = alumno.IdTutor;
            existente.Activo = alumno.Activo ?? true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno == null) return NotFound();

            alumno.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}