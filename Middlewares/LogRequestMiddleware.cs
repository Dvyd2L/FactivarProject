using Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Middlewares;

public class LogRequestMiddleware(IWebHostEnvironment env) : IMiddleware
{
    #region PROPs
    private readonly AsyncLogger _logger = new($@"{env.ContentRootPath}\wwwroot\consultas.txt");
    #endregion

    #region METHODs
    /// <summary>
    /// Invoke o InvokeAsync
    /// Este método se va a ejecutar automáticamente en cada petición porque en el program hemos registrado el middleware así:
    /// <![CDATA[app.UseMiddleware<IpBlockerMiddleware>();]]>
    /// </summary>
    /// <param name="httpContext">tiene información de la petición que viene</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        string IP = httpContext.Connection.RemoteIpAddress?.ToString() ?? "NotAvailable";
        string metodo = httpContext.Request.Method;
        string ruta = httpContext.Request.Path.ToString();

        _logger.LogMessage($"{DateTime.Now} - {IP} - {ruta} - {metodo}");

        await next(httpContext);
    }

    public void Dispose() => _logger.Dispose();
    #endregion
}
