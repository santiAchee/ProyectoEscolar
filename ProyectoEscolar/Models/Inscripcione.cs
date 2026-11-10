using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class Inscripcione
{
    public int IdInscripcion { get; set; }

    public int IdAlumno { get; set; }

    public int IdCurso { get; set; }

    public int CicloLectivo { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public string EstadoInscripcion { get; set; } = null!;

    public DateTime? FechaResolucion { get; set; }

    public string? Observacion { get; set; }

    public bool? Activo { get; set; }

    public virtual Alumno IdAlumnoNavigation { get; set; } = null!;

    public virtual Curso IdCursoNavigation { get; set; } = null!;

    public virtual ICollection<InscripcionDocumentacione> InscripcionDocumentaciones { get; set; } = new List<InscripcionDocumentacione>();
}
