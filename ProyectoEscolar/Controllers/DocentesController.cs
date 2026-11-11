using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var docentes = await _context.Docentes
                .Where(d => d.Activo == true)
                .ToListAsync();
            return Ok(docentes);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Docente docente)
        {
            if (await _context.Docentes.AnyAsync(d => d.Dni == docente.Dni))
                return BadRequest(new { mensaje = "Ya existe un docente con ese DNI." });

            if (await _context.Docentes.AnyAsync(d => d.Legajo == docente.Legajo))
                return BadRequest(new { mensaje = "Ya existe un docente con ese Legajo." });

            docente.IdDocente = 0;
            docente.Activo = true;
            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            return Ok(docente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Docente docente)
        {
            var existente = await _context.Docentes.FindAsync(id);
            if (existente == null) return NotFound(new { mensaje = "Docente no encontrado." });

            existente.Nombre = docente.Nombre;
            existente.Apellido = docente.Apellido;
            existente.Dni = docente.Dni;
            existente.Legajo = docente.Legajo;
            existente.Telefono = docente.Telefono;
            existente.Email = docente.Email;
            existente.Activo = docente.Activo ?? true;

            await _context.SaveChangesAsync();
            return NoContent();
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