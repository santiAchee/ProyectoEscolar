using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class Tutore
{
    public int IdTutor { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string Tipo { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
}
