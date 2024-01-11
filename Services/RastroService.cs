using Helpers.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;

namespace Services;
public class RastroService<TContext>(TContext context, IHttpContextAccessor accessor, IHostEnvironment hostEnvironment) : IRastroService where TContext : DbContext
{
    private readonly TContext _context = context;
    private readonly IHttpContextAccessor _accessor = accessor;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    public async Task<int> AddRastro(string usuario, EnumTipoProcesoRastro proceso, EnumTipoAccionRastro operacion, string observaciones)
    {
        if (!_hostEnvironment.IsDevelopment())
        {
            var idUsuario = (from x in _context.Usuarios
                             where x.Email == usuario
                             select x.IdUsuario).SingleOrDefault();

            Rastro rastro = new Rastro()
            {
                FechaAccion = DateTime.Now,
                Observaciones = observaciones,
                Proceso = Enum.GetName(typeof(EnumTipoProcesoRastro), proceso),
                Operacion = Enum.GetName(typeof(EnumTipoAccionRastro), operacion),
                Usuarios_IdUsuario = idUsuario,
                Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            await _context.Rastros.AddAsync(rastro);
            _ = await _context.SaveChangesAsync();
        }

        return await Task.FromResult(0);
    }
}