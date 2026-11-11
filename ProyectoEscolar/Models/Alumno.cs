using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProyectoEscolar.Models;

[Table("ALUMNOS")]
public partial class Alumno
{
    public int IdAlumno { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public int? IdTutor { get; set; }
    public bool? Activo { get; set; } = true;

    [JsonIgnore]
    public virtual Tutore? IdTutorNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}