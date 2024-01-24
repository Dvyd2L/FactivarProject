using DTOs.FactivarAPI;
using FactivarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FactivarAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientesController(FactivarContext context) : ControllerBase
{
    #region ########### PROPIEDADES ###########
    private readonly FactivarContext _context = context;
    #endregion

    #region ########### METODOS ###########
    #region GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> Get()
    {
        List<Cliente>? result = await _context.Clientes.ToListAsync();

        return result is null
         ? NotFound()
         : Ok(result);
    }

    [HttpGet("{cif}")]
    public async Task<ActionResult<Cliente>> GetClientePorCif(string cif)
    {
        Cliente? result = await _context.Clientes.FirstOrDefaultAsync(c => c.Cif == cif);

        return result is null
         ? NotFound()
         : Ok(result);
    }

    [HttpGet("{fechamin}/{fechamax}")]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClienteAltaEnRango([FromRoute] DateOnly fechamin, [FromRoute] DateOnly fechamax)
    {
        List<Cliente>? result = await _context.Clientes.Where(c => c.FechaAlta >= fechamin && c.FechaAlta <= fechamax).ToListAsync();

        return result is null
         ? NotFound()
         : Ok(result);
    }
    #endregion

    #region POST
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DTOCliente input)
    {
        if (await _context.Clientes.AnyAsync(c => c.Cif == input.Cif)) return BadRequest("El cliente ya existe");

        Cliente newCliente = new()
        {
            Cif = input.Cif,
            Nombre = input.Nombre,
            Direccion = input.Direccion,
            Telefono = input.Telefono,
            Email = input.Email,
            // Si no se pasa fecha de alta, se asigna la fecha actual.
            FechaAlta = input?.FechaAlta ?? new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
        };

        _ = await _context.Clientes.AddAsync(newCliente);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion

    #region PUT
    [HttpPut]
    public async Task<IActionResult> UpdateClient([FromBody] DTOCliente input)
    {
        Cliente? clienteDB = await _context.Clientes.FirstOrDefaultAsync(c => c.Cif == input.Cif);
        if (clienteDB == null) return BadRequest("El cliente no existe");

        clienteDB.Nombre = input.Nombre;
        clienteDB.Direccion = input.Direccion;
        clienteDB.Telefono = input.Telefono;
        clienteDB.Email = input.Email;
        clienteDB.FechaAlta = (DateOnly)input.FechaAlta;

        _ = _context.Update(clienteDB);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion

    #region DELETE
    [HttpDelete("{cif}")]
    public async Task<IActionResult> DeleteClient(string cif)
    {
        Cliente? clienteDB = await _context.Clientes.Include((c) => c.FacturaClientes).FirstOrDefaultAsync(c => c.Cif == cif);
        if (clienteDB == null) return BadRequest(new { msg = "El cliente no existe" });

        // if (clienteDB.Facturas.Count > 0) return BadRequest(new { msg = "No se pueden eliminar clientes con facturas asociadas" });
        // clienteDB.FacturaClientes.Clear(); // Elimina las facturas asociadas al cliente.
        //_ = _context.Clientes.Remove(clienteDB);

        clienteDB.Eliminado = true; // Cambia el estado del cliente a eliminado.
        _ = _context.Clientes.Update(clienteDB);
        _ = await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion
    #endregion
}
