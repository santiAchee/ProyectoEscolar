using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> Post(Tutore tutor)
        {
            var dniExiste = await _context.Tutores.AnyAsync(t => t.Dni == tutor.Dni);
            if (dniExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un tutor registrado con ese DNI." });
            }

            tutor.Activo = true;
            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();

            return Ok(tutor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Tutore tutor)
        {
            if (id != tutor.IdTutor) return BadRequest(new { mensaje = "El ID no coincide." });

            _context.Tutores.Update(tutor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            if (tutor == null) return NotFound();

            tutor.Activo = false; // Baja lógica
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}