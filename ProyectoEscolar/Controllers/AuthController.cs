using Microsoft.AspNetCore.Mvc;
using ProyectoEscolar.Models;

namespace ProyectoEscolar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Usuarios preconfigurados según los actores del Diagrama de Actividades y DFD
        private static readonly Dictionary<string, (string Password, string Rol, string NombreMostrar)> UsuariosSistema = new(StringComparer.OrdinalIgnoreCase)
        {
            { "admin", ("1234", "Administrador General", "Administración") },
            { "secretaria", ("1234", "Personal Administrativo", "Mesa de Entradas / Secretaría") },
            { "secretario", ("1234", "Secretario General", "Secretaría General") },
            { "direccion", ("1234", "Dirección Escolar", "Dirección Institucional") }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { mensaje = "Debe ingresar usuario y contraseña." });

            if (UsuariosSistema.TryGetValue(request.Usuario.Trim(), out var datos) && datos.Password == request.Password)
            {
                // Generamos un token de sesión amigable que no bloquea Swagger
                var fakeToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{request.Usuario}:{datos.Rol}:{DateTime.UtcNow.Ticks}"));

                return Ok(new
                {
                    token = fakeToken,
                    usuario = request.Usuario.ToLower(),
                    rol = datos.Rol,
                    nombre = datos.NombreMostrar,
                    mensaje = $"Bienvenido/a {datos.NombreMostrar}"
                });
            }

            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos. Probá con: admin / 1234" });
        }
    }
}