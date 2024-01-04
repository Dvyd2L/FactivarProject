using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

/// <summary>
/// Clase que proporciona funcionalidades para generar tokens de autenticación.
/// </summary>
/// <param name="configuration">Una instancia de IConfiguration para acceder a la configuración de la aplicación.</param>
public class TokenService(IConfiguration configuration)
{
    /// <summary>
    /// Una instancia de IConfiguration para acceder a la configuración de la aplicación.
    /// </summary>
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Genera un token de autenticación para un usuario.
    /// </summary>
    /// <param name="credenciales">Las credenciales del usuario para el cual se generará el token. 
    /// Debe contener el correo electrónico en el primer índice y el rol en el segundo índice.</param>
    /// <returns>Una respuesta de inicio de sesión que contiene el correo electrónico del usuario y el token de autenticación.</returns>
    public string GenerarToken(params string[] credenciales)
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

        foreach (string item in credenciales.Skip(2))
        {
            claims.Add(new Claim(nameof(item), item));
        }

        // Necesitamos la clave, audiencia y emisor de generación de tokens
        string clave = _configuration["ClaveJWT"] ?? "";
        string issuer = _configuration["IssuerJWT"] ?? "";
        string audience = _configuration["AudienceJWT"] ?? "";

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

        return tokenString;
    }
}
