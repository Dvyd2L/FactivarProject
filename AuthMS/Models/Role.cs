using System;
using System.Collections.Generic;

namespace AuthMS.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Credenciale> Credenciales { get; set; } = new List<Credenciale>();
}
