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
    public class CursosController : ControllerBase
    {
        private readonly SistemaEscolarContext _context;

        public CursosController(SistemaEscolarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cursos = await _context.Cursos
                .Where(c => c.Activo == true)
                .ToListAsync();
            return Ok(cursos);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Curso curso)
        {
            // Validar que el Docente a cargo exista
            var docenteExiste = await _context.Docentes.AnyAsync(d => d.IdDocente == curso.IdDocente);
            if (!docenteExiste)
            {
                return BadRequest(new { mensaje = "El ID de docente especificado no existe." });
            }

            curso.Activo = true;
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            return Ok(curso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Curso curso)
        {
            if (id != curso.IdCurso) return BadRequest(new { mensaje = "El ID no coincide." });

            var docenteExiste = await _context.Docentes.AnyAsync(d => d.IdDocente == curso.IdDocente);
            if (!docenteExiste)
            {
                return BadRequest(new { mensaje = "El ID de docente especificado no existe." });
            }

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            curso.Activo = false; // Baja lógica
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}