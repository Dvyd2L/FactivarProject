using Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Helpers;

public class IsAdmin : Attribute, IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
        ResourceExecutionDelegate next)
    {
        try
        {
            Microsoft.Extensions.Primitives.StringValues headerAuthorizationValue = context.HttpContext.Request.Headers.ToList().Find(x => x.Key == "Authorization").Value;
            string token = headerAuthorizationValue.ToString().Split(" ").Last();
            JwtSecurityTokenHandler handler = new();
            Microsoft.IdentityModel.Tokens.SecurityToken jsonToken = handler.ReadToken(token);
            JwtSecurityToken? tokenParsed = jsonToken as JwtSecurityToken;
            System.Security.Claims.Claim? rol = tokenParsed?.Claims.ToList().FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            if (rol?.Value != EnumRoles.Admin.ToString())
            {
                context.Result = new BadRequestResult();
                _ = await next();
            }
            else
            {
                _ = await next(); // Sigue hacia adelante
                return; // Detiene la revisión del filtro
            }
        }
        catch (Exception)
        {
            context.Result = new BadRequestResult();
            _ = await next();
        }
    }
}
