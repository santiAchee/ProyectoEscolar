using System;
using System.Collections.Generic;

namespace ProyectoEscolar.Models;

public partial class Documentacione
{
    public int IdDocumentacion { get; set; }

    public int IdTipoDocumento { get; set; }

    public DateOnly? FechaEmision { get; set; }

    public virtual TiposDocumento IdTipoDocumentoNavigation { get; set; } = null!;

    public virtual ICollection<InscripcionDocumentacione> InscripcionDocumentaciones { get; set; } = new List<InscripcionDocumentacione>();
}
