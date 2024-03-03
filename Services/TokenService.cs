using DTOs.FactivarAPI;
using DTOs.UsersMS;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
  public string GenerarToken(UserDTO user, params string[] credenciales)
  {
    DateTime expirationTime = DateTime.Now.AddDays(30); // tiempo de expiracion

    // Los claims construyen la información que va en el payload del token
    List<Claim> claims =
    [
      new Claim(nameof(ClaimTypes.Sid).ToLower(), user.Id.ToString()),
      new Claim(nameof(ClaimTypes.Email).ToLower(), user.Email),
      new Claim(nameof(ClaimTypes.Name).ToLower(), user.Nombre),
      new Claim(nameof(ClaimTypes.Surname).ToLower(), user.Apellidos),
      new Claim(nameof(ClaimTypes.Role).ToLower(), user.IsAdmin ? "Admin" : "User"),
    ];

    if (user.AvatarUrl is not null)
    {
      claims.Add(new Claim(nameof(ClaimTypes.Thumbprint).ToLower(), user.AvatarUrl));
    }

    if (user.Telefono is not null)
    {
      claims.Add(new Claim(nameof(ClaimTypes.MobilePhone).ToLower(), user.Telefono.Trim()));
    }

    foreach (string item in credenciales)
    {
      claims.Add(new Claim("Claims", item));
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

  public string GenerarTokenArticulos(List<DTOArticulo> articulos)
  {
    List<Claim> claims = [];
    //List<Claim> claims =
    //[
    //    new Claim("ListaArticulos", JsonConvert.SerializeObject(articulos))
    //];

    foreach (DTOArticulo art in articulos)
    {
      claims.Add(new Claim(art.Descripcion, JsonConvert.SerializeObject(art)));
    }

    string clave = _configuration["ClaveJWT"] ?? "";
    string issuer = _configuration["IssuerJWT"] ?? "";
    string audience = _configuration["AudienceJWT"] ?? "";

    SymmetricSecurityKey claveKey = new(Encoding.UTF8.GetBytes(clave));
    SigningCredentials signinCredentials = new(claveKey, SecurityAlgorithms.HmacSha256);

    JwtSecurityToken securityToken = new(
        issuer: issuer,
        audience: audience,
        claims: claims,
        signingCredentials: signinCredentials
    );

    string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

    return tokenString;
  }

  public List<DTOArticulo> LeerTokenArticulos(string token)
  {
    IEnumerable<Claim> variables = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;
    List<DTOArticulo> articulos = [];
    foreach (Claim? a in variables.SkipLast(2))
    {
      articulos.Add(JsonConvert.DeserializeObject<DTOArticulo>(a.Value) ?? new DTOArticulo());
    }

    return articulos;
  }
}
