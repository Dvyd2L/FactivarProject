using System;
using System.Collections.Generic;

namespace FactivarAPI.Models;

public partial class Factura
{
    public int NumeroFactura { get; set; }

    public decimal Importe { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public bool PendientePago { get; set; }

    public string DescripcionOperacion { get; set; } = null!;

    public DateOnly FechaExpedicion { get; set; }

    public DateOnly? FechaCobro { get; set; }

    public string ClienteId { get; set; } = null!;

    public string Articulos { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;
}
