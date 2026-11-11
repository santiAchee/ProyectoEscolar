using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutoresController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public TutoresController(SistemaEscolarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tutores = await _context.Tutores
                .Where(t => t.Activo == true)
                .ToListAsync();
            return Ok(tutores);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Tutore tutor)
        {
            if (await _context.Tutores.AnyAsync(t => t.Dni == tutor.Dni))
                return BadRequest(new { mensaje = "Ya existe un tutor con ese DNI." });

            tutor.IdTutor = 0;
            tutor.Activo = true;
            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();

            return Ok(tutor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tutore tutor)
        {
            var existente = await _context.Tutores.FindAsync(id);
            if (existente == null) return NotFound(new { mensaje = "Tutor no encontrado." });

            existente.Nombre = tutor.Nombre;
            existente.Apellido = tutor.Apellido;
            existente.Dni = tutor.Dni;
            existente.Telefono = tutor.Telefono;
            existente.Email = tutor.Email;
            existente.Tipo = tutor.Tipo;
            existente.Activo = tutor.Activo ?? true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            if (tutor == null) return NotFound();

            tutor.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}