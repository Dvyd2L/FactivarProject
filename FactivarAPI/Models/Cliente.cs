using System;
using System.Collections.Generic;

namespace FactivarAPI.Models;

public partial class Cliente
{
    public string Cif { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public int Telefono { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly FechaAlta { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
