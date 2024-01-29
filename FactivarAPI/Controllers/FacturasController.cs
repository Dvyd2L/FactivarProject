using DTOs.FactivarAPI;
using FactivarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Services;

namespace FactivarAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FacturasController(FactivarContext context, CalculoIvaService calculoIvaService, TokenService tokenService) : ControllerBase
{
    #region ########### PROPIEDADES ###########
    private readonly FactivarContext _context = context;
    private readonly CalculoIvaService _calculoIvaService = calculoIvaService;
    private readonly TokenService _tokenService = tokenService;
    #endregion

    #region ########### METODOS ###########
    private IEnumerable<DTOIvas> IvasCalculados(List<Factura> input)
    {
        List<DTOArticulo> articulosFinal = [];
        List<DTOArticulo>? articulos;

        string articulosFactura;
        foreach (Factura f in input)
        {
            articulosFactura = f.Articulos;
            articulos = JsonConvert.DeserializeObject<List<DTOArticulo>>(articulosFactura);
            articulosFinal.AddRange(articulos!);
        }

        IEnumerable<DTOs.FactivarAPI.DTOIvas> ivaMensual = _calculoIvaService.CalculoIVA(articulosFinal);

        return ivaMensual;
    }

    #region GET
    [HttpGet("{pk:int}")]
    public async Task<ActionResult<DTOFacturaResponse?>> GetByPk([FromRoute] int pk)
    {
        Factura? result = await _context.Facturas
            .Include((f) => f.Cliente).Include((f) => f.Proveedor)
            .FirstOrDefaultAsync((f) => f.FacturaId == pk);

        if (result is null)
        {
            return NotFound(new { msg = "No se ha encontrado la factura" });
        }

        List<DTOArticulo> articulos = JsonConvert.DeserializeObject<List<DTOArticulo>>(result.Articulos) ?? [];

        DTOFacturaResponse response = new()
        {
            NumeroFactura = result.NumeroFactura,
            Cliente = new DTOCliente()
            {
                Cif = result.Cliente.Cif,
                Nombre = result.Cliente.Nombre,
                Direccion = result.Cliente.Direccion,
                Email = result.Cliente.Email,
                FechaAlta = result.Cliente.FechaAlta,
                Telefono = result.Cliente.Telefono,
            },
            Proveedor = new DTOCliente()
            {
                Cif = result.Proveedor.Cif,
                Nombre = result.Proveedor.Nombre,
                Direccion = result.Proveedor.Direccion,
                Email = result.Proveedor.Email,
                FechaAlta = result.Proveedor.FechaAlta,
                Telefono = result.Proveedor.Telefono,
            },
            Articulos = articulos,
            CalculosIvas = _calculoIvaService.CalculoIVA(articulos),
            DesgloseIva = _calculoIvaService.DesgloseIVA(articulos),
            DescripcionOperacion = result.DescripcionOperacion,
            FechaExpedicion = result.FechaExpedicion,
            FechaCobro = result.FechaCobro,
            PendientePago = result.PendientePago,
        };

        return Ok(response);
    }

    [HttpGet("cliente/{cif}")]
    public async Task<IActionResult> GetFacturasDeCliente([FromRoute] string cif)
    {
        Cliente? clienteDB = await _context.Clientes.FirstOrDefaultAsync(c => c.Cif == cif);
        if (clienteDB == null) return BadRequest("El cliente no existe");

        List<Factura>? result = await _context.Facturas.Where(f => f.ClienteId == cif).ToListAsync();

        return result is null
         ? NotFound()
         : Ok(result);
    }

    //[HttpGet("acotadoFecha/{cliente}/{fechaMin}/{fechaMax}")]
    //public async Task<IActionResult> GetFacturasDeCliente(string cliente, DateOnly fechaMin, DateOnly fechaMax)
    //{
    //    var clienteDB = await _context.Clientes.FirstOrDefaultAsync(c => c.Cif == cliente);
    //    if (clienteDB == null) return BadRequest("El cliente no existe");

    //    var result = await _context.Facturas.Where(f => f.ClienteId == cliente &&
    //                                                    f.FechaExpedicion >= fechaMin &&
    //                                                    f.FechaExpedicion <= fechaMax).ToListAsync();

    //    return result is null
    //     ? NotFound()
    //     : Ok(result);
    //}

    [HttpGet("ivamensual/{cliente}/{mes}/{year}")]
    public async Task<IActionResult> GetIvaMensual([FromRoute] string cliente, [FromRoute] int mes, [FromRoute] int year)
    {
        List<Factura> facturasMes = await _context.Facturas.Where(f => f.ClienteId == cliente && f.FechaExpedicion.Month == mes && f.FechaExpedicion.Year == year).ToListAsync();
        if (facturasMes.Count == 0) return BadRequest("No se han encontrado facturas en el mes indicado");

        IEnumerable<DTOIvas> ivaMensual = IvasCalculados(facturasMes);

        return Ok(ivaMensual);
    }

    [HttpGet("ivatrimestral/{cliente}/{trimestre}/{year}")]
    public async Task<IActionResult> GetIvatrimestral([FromRoute] string cliente, [FromRoute] int year, [FromRoute] int trimestre)
    {
        int mesMin = 0, mesMax = 0;
        switch (trimestre)
        {
            case 1:
                mesMin = 1;
                mesMax = 3;
                break;
            case 2:
                mesMin = 4;
                mesMax = 6;
                break;
            case 3:
                mesMin = 7;
                mesMax = 9;
                break;
            case 4:
                mesMin = 10;
                mesMax = 12;
                break;
        }

        List<Factura> facturasMes = await _context.Facturas.Where(f => f.ClienteId == cliente &&
                                                             f.FechaExpedicion.Month >= mesMin &&
                                                             f.FechaExpedicion.Month <= mesMax &&
                                                             f.FechaExpedicion.Year == year).ToListAsync();
        if (facturasMes.Count == 0) return BadRequest("No se han encontrado facturas en el trimestre indicado");

        IEnumerable<DTOIvas> ivaMensual = IvasCalculados(facturasMes);

        return Ok(ivaMensual);
    }

    [HttpGet("ivaanual/{cliente}/{year}")]
    public async Task<IActionResult> GetIvaAnual(string cliente, int year)
    {
        List<Factura> facturasMes = await _context.Facturas.Where(f => f.ClienteId == cliente && f.FechaExpedicion.Year == year).ToListAsync();
        if (facturasMes.Count == 0) return BadRequest("No se han encontrado facturas en el año indicado");

        IEnumerable<DTOIvas> ivaMensual = IvasCalculados(facturasMes);

        return Ok(ivaMensual);
    }

    [HttpGet("pendientesDePagoPorCliente/{cliente}")]
    public async Task<IActionResult> GetPendientesDePagoPorCliente([FromRoute] string cliente)
    {
        List<Factura>? result = await _context.Facturas.Where(f => f.PendientePago && f.ClienteId == cliente).ToListAsync();

        return result is null
         ? NotFound()
         : Ok(result);
    }

    //[HttpGet("pendientesDePagoPorProveedor/{proveedor}")]
    //public async Task<IActionResult> GetPendientesDePagoPorProveedor(string proveedor)
    //{
    //    var result = await _context.Facturas.Where(f => f.PendientePago && f.ProveedorId == proveedor).ToListAsync();

    //    return result is null
    //     ? NotFound()
    //     : Ok(result);
    //}
    #endregion

    #region POST
    [HttpPost("prueba-token")]
    public string Pruebatoken([FromBody] List<DTOArticulo> input)
    {
        string result = _tokenService.GenerarTokenArticulos(input);
        return result;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] DTOFactura input)
    {
        if (await _context.Facturas.AnyAsync(f => f.NumeroFactura == input.NumeroFactura))
            return BadRequest("Numero de factura ya existente");

        Factura newFactura = new()
        {
            NumeroFactura = input.NumeroFactura,
            PendientePago = input.PendientePago,
            DescripcionOperacion = input.DescripcionOperacion,
            FechaExpedicion = input.FechaExpedicion,
            FechaCobro = input.FechaCobro,
            ClienteId = input.ClienteId,
            ProveedorId = input.ProveedorId,
            Articulos = JsonConvert.SerializeObject(input.Articulos)
        };

        _ = await _context.Facturas.AddAsync(newFactura);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }

    //[HttpPost]
    //public async Task<ActionResult> Post([FromBody] DTOFactura input)
    //{
    //    if (await _context.Facturas.AnyAsync(f => f.NumeroFactura == input.NumeroFactura)) return BadRequest("Numero de factura ya existente");

    //    var newFactura = new Factura()
    //    {
    //        NumeroFactura = input.NumeroFactura,
    //        //Importe = input.Importe,
    //        //Iva = input.Iva,
    //        //Total = input.Total,
    //        PendientePago = input.PendientePago,
    //        DescripcionOperacion = input.DescripcionOperacion,
    //        FechaExpedicion = input.FechaExpedicion,
    //        FechaCobro = input.FechaCobro,
    //        ClienteId = input.ClienteId,
    //        Articulos = JsonConvert.SerializeObject(input.Articulos)
    //    };

    //    var totalesArticulos = ivasCalculados(new List<Factura> { newFactura });
    //    newFactura.Importe = totalesArticulos.Sum(total => total.BImponible);
    //    newFactura.Iva = totalesArticulos.Sum(total => total.Cuota);

    //    await _context.Facturas.AddAsync(newFactura);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}
    #endregion

    #region PUT
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] DTOFactura input)
    {
        Factura? facturaDB = await _context.Facturas.FirstOrDefaultAsync(f => f.NumeroFactura.Equals(input.NumeroFactura));
        if (facturaDB == null) return BadRequest("La factura no existe");

        facturaDB.PendientePago = input.PendientePago;
        facturaDB.DescripcionOperacion = input.DescripcionOperacion;
        facturaDB.FechaCobro = input.FechaCobro;

        _ = _context.Facturas.Update(facturaDB);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("pagado/{nFactura}")]
    public async Task<IActionResult> Put([FromRoute] int nFactura)
    {
        Factura? facturaDB = await _context.Facturas.FirstOrDefaultAsync(f => f.NumeroFactura == nFactura);
        if (facturaDB == null) return BadRequest("La factura no existe");

        if (!facturaDB.PendientePago) return BadRequest("Factura pagada");

        facturaDB.PendientePago = false;
        facturaDB.FechaCobro = DateOnly.FromDateTime(DateTime.Now);

        _ = _context.Facturas.Update(facturaDB);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion

    #region DELETE
    [HttpDelete("{nFactura}")]
    public async Task<IActionResult> DeleteClient([FromRoute] int nFactura)
    {
        Factura? facturaDB = await _context.Facturas.FirstOrDefaultAsync(f => f.NumeroFactura == nFactura);
        if (facturaDB == null) return BadRequest("La factura no existe");

        _ = _context.Facturas.Remove(facturaDB);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion

    #endregion
}
