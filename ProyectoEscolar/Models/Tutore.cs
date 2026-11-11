using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProyectoEscolar.Models;

[Table("TUTORES")]
public partial class Tutore
{
    public int IdTutor { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string Tipo { get; set; } = null!; // 'Padre', 'Madre', 'Tutor Legal'
    public bool? Activo { get; set; } = true;

    [JsonIgnore]
    public virtual ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
}