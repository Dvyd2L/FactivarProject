namespace DTOs.FactivarAPI;

public class DTOArticulo
{
    public string Descripcion { get; set; } = null!;
    public int Unidades { get; set; }
    public decimal PUnitario { get; set; }
    public IVA Iva { get; set; }
    public decimal BImponible => Unidades * PUnitario;
    public decimal CuotaIva => BImponible * (int)Iva / 100;
    public decimal Importe => BImponible * CuotaIva;
}

public class DTOIvas
{
    public decimal BImponible { get; set; } = 0;
    public decimal Cuota { get; set; } = 0;
    public decimal Total => BImponible + Cuota;
}

public enum IVA
{
    CERO = 0,
    SUPERREDUCIDO = 4,
    ESPECIAL = 5,
    REDUCIDO = 10,
    GENERAL = 21,
}
