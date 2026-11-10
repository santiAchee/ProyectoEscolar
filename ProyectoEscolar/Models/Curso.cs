using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class Curso
{
    public int IdCurso { get; set; }

    public int IdDocente { get; set; }

    public string Nivel { get; set; } = null!;

    public int AnioCurso { get; set; }

    public string Division { get; set; } = null!;

    public string Turno { get; set; } = null!;

    public int CupoMaximo { get; set; }

    public bool? Activo { get; set; }

    public virtual Docente IdDocenteNavigation { get; set; } = null!;

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();
}
