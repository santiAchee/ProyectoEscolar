using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class TiposDocumento
{
    public int IdTipoDocumento { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Obligatorio { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Documentacione> Documentaciones { get; set; } = new List<Documentacione>();
}
