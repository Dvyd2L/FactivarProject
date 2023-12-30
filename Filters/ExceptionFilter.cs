using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters;

public class ExceptionFilter(IWebHostEnvironment env)
    : ExceptionFilterAttribute
{
    private readonly string _logFilePath = $@"{env.ContentRootPath}\wwwroot\logErrores.txt";

    public override void OnException(ExceptionContext context)
    {
        string IP = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        string metodo = context.HttpContext.Request.Method;
        string ruta = context.HttpContext.Request.Path.ToString();
        string error = context.Exception.Message;

        using (StreamWriter writer = new(_logFilePath, append: true))
        {
            writer.WriteLine($@"
IP: {IP,25}
Método: {metodo,25}
Ruta: {ruta,25}
Error: {error,25}
Fecha: {DateTime.Now,25}");
        }

        base.OnException(context);
    }
}
