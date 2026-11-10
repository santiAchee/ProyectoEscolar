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
    public class DocentesController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public DocentesController(SistemaEscolarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var docentes = await _context.Docentes
                    .Where(d => d.Activo == true)
                    .ToListAsync();
                return Ok(docentes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en servidor", detalle = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Docente docente)
        {
            try
            {
                // Validación simplificada y segura
                docente.Activo = true;
                _context.Docentes.Add(docente);
                await _context.SaveChangesAsync();

                return Ok(docente);
            }
            catch (Exception ex)
            {
                // Esto te devolverá el error exacto de SQL / Entity Framework a la pantalla o consola
                return StatusCode(500, new { mensaje = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Docente docente)
        {
            if (id != docente.IdDocente) return BadRequest(new { mensaje = "El ID no coincide." });

            try
            {
                _context.Docentes.Update(docente);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null) return NotFound();

            docente.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}