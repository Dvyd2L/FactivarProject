using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Middlewares;

/// <summary>
/// La clase PreflightRequestMiddleware es un middleware personalizado que se utiliza para manejar las solicitudes CORS.
/// Añade los encabezados CORS necesarios a la respuesta del servidor y maneja las solicitudes de tipo OPTIONS. 
/// </summary>
public class PreflightRequestMiddleware : IMiddleware
{
    #region PROPs
    /// <summary>
    /// Los métodos HTTP permitidos en las solicitudes CORS.
    /// </summary>
    private static readonly string[] _httpMethods = ["GET, POST, PUT, DELETE, OPTIONS"];

    /// <summary>
    /// Los encabezados permitidos en las solicitudes CORS.
    /// </summary>
    private static readonly string[] _httpHeaders = ["Origin, X-Requested-With, Content-Type, Accept, Authorization, ActualUserOrImpersonatedUserSamAccount, IsImpersonatedUser"];

    /// <summary>
    /// Indica si se permiten credenciales en las solicitudes CORS.
    /// </summary>
    private static readonly string[] _httpCredentials = ["true"];

    /// <summary>
    /// Los orígenes permitidos en las solicitudes CORS.
    /// </summary>
    private static readonly string[] _allowOrigins = ["*"];
    #endregion PROPs

    #region METHODs
    /// <summary>
    /// Invoca el middleware personalizado para manejar las solicitudes CORS.
    /// </summary>
    /// <param name="next">Funcion que procesa peticiones http, se usa para dar paso al siguiente middleware</param>
    /// <param name="context">El contexto HTTP actual.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next) => await BeginInvoke(context, next);

    /// <summary>
    /// Este método es un middleware personalizado para manejar las solicitudes CORS.
    /// Añade los encabezados CORS necesarios a la respuesta del servidor y maneja las solicitudes de tipo OPTIONS.
    /// </summary>
    /// <param name="next">Funcion que procesa peticiones http, se usa para dar paso al siguiente middleware</param>
    /// <param name="context">El contexto HTTP actual.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task BeginInvoke(HttpContext context, RequestDelegate next)
    {
        // Añade los encabezados CORS a la respuesta del servidor
        context.Response.Headers.Append(HeaderNames.AccessControlAllowCredentials, _httpCredentials);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowHeaders, _httpHeaders);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowMethods, _httpMethods);
        context.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, _allowOrigins);

        // Maneja las solicitudes de tipo OPTIONS
        if (context.Request.Method == HttpMethod.Options.Method)
        {
            string msg = "Preflight request";

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(msg);
        }

        // Invoca el siguiente middleware en la cadena
        await next(context);
    }
    #endregion METHODs
}
