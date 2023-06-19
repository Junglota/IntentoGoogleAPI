using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Tipo
{
    public int IntId { get; set; }

    public string? Tipo1 { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
