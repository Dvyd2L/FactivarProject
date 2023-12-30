using Helpers;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class TokenService(IConfiguration configuration)
{
    public ILoginResponse GenerarToken(params string[] credenciales)
    {
        string email = credenciales[0];
        string rol = credenciales[1] ??= "USER";
        DateTime expirationTime = DateTime.Now.AddDays(30); // tiempo de expiracion

        // Los claims construyen la información que va en el payload del token
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rol),
        ];

        // Necesitamos la clave, audiencia y emisor de generación de tokens
        string clave = configuration["ClaveJWT"] ?? "";
        string issuer = configuration["IssuerJWT"] ?? "";
        string audience = configuration["AudienceJWT"] ?? "";

        // Fabricamos el token
        SymmetricSecurityKey claveKey = new(Encoding.UTF8.GetBytes(clave));
        SigningCredentials signinCredentials = new(claveKey, SecurityAlgorithms.HmacSha256);

        // Le damos características
        JwtSecurityToken securityToken = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expirationTime,
            signingCredentials: signinCredentials
        );

        // Lo pasamos a string para devolverlo
        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return new LoginResponse(email, tokenString);
    }
}
