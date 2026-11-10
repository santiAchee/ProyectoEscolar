using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class InscripcionDocumentacione
{
    public int IdInscripcion { get; set; }

    public int IdDocumentacion { get; set; }

    public bool Presentado { get; set; }

    public DateTime? FechaPresentacion { get; set; }

    public bool Validado { get; set; }

    public DateTime? FechaValidacion { get; set; }

    public string? Observaciones { get; set; }

    public virtual Documentacione IdDocumentacionNavigation { get; set; } = null!;

    public virtual Inscripcione IdInscripcionNavigation { get; set; } = null!;
}
