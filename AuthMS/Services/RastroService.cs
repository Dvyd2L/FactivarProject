using AuthMS.Models;
using Helpers.Enums;
using Services.Interfaces;

namespace AuthMS.Services;
public class RastroService(AuthContext context, IHttpContextAccessor accessor, IHostEnvironment hostEnvironment) : IRastroService
{
    private readonly AuthContext _context = context;
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    public async Task<int> AddRastro(string usuario, EnumTipoProcesoRastro proceso, EnumTipoAccionRastro operacion, string observaciones)
    {
        if (!_hostEnvironment.IsDevelopment())
        {
            Guid idUsuario = (from x in _context.DatosPersonales
                              where x.Email == usuario
                              select x.Id).SingleOrDefault();

            Registro log = new()
            {
                FechaAccion = DateTime.Now,
                Observaciones = observaciones,
                Proceso = Enum.GetName(proceso),
                Operacion = Enum.GetName(operacion),
                Usuarios_IdUsuario = idUsuario,
                Ip = _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            };

            _ = await _context.Registros.AddAsync(log);
            _ = await _context.SaveChangesAsync();
        }

        return await Task.FromResult(0);
    }
}