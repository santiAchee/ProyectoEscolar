using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProyectoEscolar.Models;

[Table("DOCENTES")]
public partial class Docente
{
    public int IdDocente { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string Legajo { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool? Activo { get; set; } = true;

    [JsonIgnore]
    public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}