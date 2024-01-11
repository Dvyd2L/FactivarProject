using System;
using System.Collections.Generic;

namespace AuthMS.Models;

public partial class Registro
{
    public Guid IdRegistro { get; set; }

    public DateTime FechaAccion { get; set; }

    public string Proceso { get; set; } = null!;

    public string Operacion { get; set; } = null!;

    public string? Observaciones { get; set; }

    public Guid? Usuarios_IdUsuario { get; set; }

    public string? Ip { get; set; }
}
