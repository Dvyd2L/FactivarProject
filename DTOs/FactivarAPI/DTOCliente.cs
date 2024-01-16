namespace DTOs.FactivarAPI;

public class DTOCliente
{
    public string Cif { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public int Telefono { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly? FechaAlta { get; set; }
}
