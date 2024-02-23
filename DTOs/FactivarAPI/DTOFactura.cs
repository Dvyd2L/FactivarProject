namespace DTOs.FactivarAPI;

public class DTOFactura
{
    public int NumeroFactura { get; set; }
    public bool PendientePago { get; set; }
    public string DescripcionOperacion { get; set; } = null!;
    public DateOnly FechaExpedicion { get; set; }
    public DateOnly FechaCobro { get; set; }
    public string ClienteId { get; set; } = null!;
    public string ProveedorId { get; set; } = null!;
    public List<DTOArticulo> Articulos { get; set; } = null!;
}

public class DTOFacturaResponse
{
    public int NumeroFactura { get; set; }
    public IEnumerable<DTOIvas>? CalculosIvas { get; set; }
    //public Dictionary<IVA, DTOIvas>? DesgloseIva { get; set; }
    public Dictionary<string, DTOIvas>? DesgloseIva { get; set; }
    public bool PendientePago { get; set; }
    public string DescripcionOperacion { get; set; } = null!;
    public DateOnly FechaExpedicion { get; set; }
    public DateOnly? FechaCobro { get; set; }
    public DTOCliente Cliente { get; set; } = null!;
    public DTOCliente Proveedor { get; set; } = null!;
    public List<DTOArticulo> Articulos { get; set; } = null!;
    public decimal Importe => Articulos.Sum(x => x.BImponible);  //Suma de todos los precios de productos
    public decimal Iva => Articulos.Sum(x => x.CuotaIva);  //Suma de todos los ivas de productos
    public decimal Total => Importe + Iva;  //Precio final = Importe + Iva
}
/* 
p1 10€ 1u 5% ivaPrecio = 10*1 *0.05 = a€
¡2 1€ 10u 10% ivaPrecio = 1*10 *0.10  = b€
p3 20€ 5u 21% ivaPrecio =  20*5 *0.21  = c€
p4 3€ 8u 4% ivaPrecio = 3*8 *0.04  = d€

importe =  10*1 + 1*10 + 20*5 + 3*8 = sub€
IVA = a€ + b€ + c€ + d€ = iva€
total = sub€ + iva€
*/

/* token para guardar los productos como string en la base de datos */