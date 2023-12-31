using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Middlewares;

public class LogRequestMiddleware(RequestDelegate next, IWebHostEnvironment env)
{
    #region PROPs
    private readonly string _logFile = $@"{env.ContentRootPath}\wwwroot\consultas.txt";
    //private readonly string _IPv6Baneada = "::1";
    #endregion

    #region METHODs
    /// <summary>
    /// Invoke o InvokeAsync
    /// Este método se va a ejecutar automáticamente en cada petición porque en el program hemos registrado el middleware así:
    /// <![CDATA[app.UseMiddleware<IpBlockerMiddleware>();]]>
    /// </summary>
    /// <param name="httpContext">tiene información de la petición que viene</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        string IP = httpContext.Connection.RemoteIpAddress?.ToString() ?? "NotAvailable";
        string metodo = httpContext.Request.Method;
        string ruta = httpContext.Request.Path.ToString();

        await LogWriter(IP, ruta, metodo);

        await next(httpContext);
    }

    private async Task LogWriter(string IP, string path, string method)
    {
        using (StreamWriter writer = new(_logFile, append: true))
        {
            await writer.WriteLineAsync($"{DateTime.Now} - {IP} - {path} - {method}");
        }
    }
    #endregion
}
