using System;
using System.Collections.Generic;

namespace UsersMicroservice.Models;

public partial class Credenciale
{
    public Guid IdUsuario { get; set; }

    public string Password { get; set; } = null!;

    public byte[]? Salt { get; set; }

    public string? EnlaceCambioPass { get; set; }

    public DateTime? FechaEnvioEnlace { get; set; }

    public virtual DatosPersonale IdUsuarioNavigation { get; set; } = null!;
}
