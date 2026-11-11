using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProyectoEscolar.Models;

[Table("CURSOS")]
public partial class Curso
{
    public int IdCurso { get; set; }
    public int IdDocente { get; set; }
    public string Nivel { get; set; } = null!; // 'Inicial', 'Primaria', 'Secundaria'
    public int AnioCurso { get; set; }
    public string Division { get; set; } = null!;
    public string Turno { get; set; } = null!; // 'Mañana', 'Tarde'
    public int CupoMaximo { get; set; }
    public bool? Activo { get; set; } = true;

    [JsonIgnore]
    public virtual Docente? IdDocenteNavigation { get; set; }

    [JsonIgnore]
    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}