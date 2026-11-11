using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> Post([FromBody] Curso curso)
        {
            var docenteExiste = await _context.Docentes.AnyAsync(d => d.IdDocente == curso.IdDocente && d.Activo == true);
            if (!docenteExiste)
                return BadRequest(new { mensaje = "El docente especificado no existe o está inactivo." });

            curso.IdCurso = 0;
            curso.Activo = true;
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            return Ok(curso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Curso curso)
        {
            var existente = await _context.Cursos.FindAsync(id);
            if (existente == null) return NotFound(new { mensaje = "Curso no encontrado." });

            var docenteExiste = await _context.Docentes.AnyAsync(d => d.IdDocente == curso.IdDocente && d.Activo == true);
            if (!docenteExiste)
                return BadRequest(new { mensaje = "El docente especificado no existe o está inactivo." });

            existente.IdDocente = curso.IdDocente;
            existente.Nivel = curso.Nivel;
            existente.AnioCurso = curso.AnioCurso;
            existente.Division = curso.Division;
            existente.Turno = curso.Turno;
            existente.CupoMaximo = curso.CupoMaximo;
            existente.Activo = curso.Activo ?? true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            curso.Activo = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}