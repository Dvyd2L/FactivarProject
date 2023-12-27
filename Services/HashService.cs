using Helpers;
using Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services;

/// <summary>
/// Un hash es una clave que no se puede revertir.Es lo correcto para contraseñas seguras
/// El hash es lo que se guardará en la tabla de usuarios.
/// Las funciones que generan hash también nos van a servir para contrastarlos
/// Un salt es un valor aleatorio que se anexa al texto plano al que queremos aplicar la función que genera el hash
/// Añade más seguridad porque, uniendo un salt aleatorio al password, los valores siempre serán diferentes
/// Si generamos el password sin salt, basándonos solo en el password (que es solo un texto plano) los hashes generados basados en ese password siempre serán iguales
/// El salt se debe guardar junto al password para contrastar el login
/// </summary>
public class HashService(IConfiguration configuration) : IHashService
{
    #region PROPs
    private readonly IConfiguration _configuration = configuration;
    #endregion

    #region METHODs
    /// <summary>
    /// Método para generar el salt aleatorio
    /// </summary>
    /// <returns>El <![CDATA[byte[]]]> salt aleatorio</returns>
    public byte[] GetSalt()
    {
        byte[] salt = new byte[16];
        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetBytes(salt); // Genera un array aleatorio de bytes
        }

        return salt;
    }

    /// <summary>
    /// Método para obtener el hash
    /// </summary>
    /// <param name="textoPlano">Texto que deseamos encriptar</param>
    /// <param name="salt">Salt con el cual queremos generar el hash</param>
    /// <returns>Un objeto <![CDATA[IHashResult]]> con el hash y el salt que se ha usado para generarlo</returns>
    public IHashResult GetHash(string textoPlano, byte[]? salt = null)
    {
        // Si no se proporciona un salt generamos el salt aleatorio
        salt ??= GetSalt();

        //Pbkdf2 es un algoritmo de encriptación
        byte[] claveDerivada = KeyDerivation.Pbkdf2(password: textoPlano,
            salt: salt, prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 32);

        string hash = Convert.ToBase64String(claveDerivada);

        // Llamamos al record ResultadoHash y retornamos el hash con el salt
        return new HashResult(hash, salt);
    }

    public ILoginResponse GenerarToken(params string[] credenciales)
    {
        string email = credenciales[0];
        // Los claims construyen la información que va en el payload del token
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Email, email),
            new Claim("lo que yo quiera", "cualquier otro valor")
        ];

        // Necesitamos la clave de generación de tokens
        string? clave = _configuration["ClaveJWT"];

        // Fabricamos el token
        SymmetricSecurityKey claveKey = new(Encoding.UTF8.GetBytes(clave));
        SigningCredentials signinCredentials = new(claveKey, SecurityAlgorithms.HmacSha256);

        // Le damos características
        JwtSecurityToken securityToken = new(
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: signinCredentials
        );

        // Lo pasamos a string para devolverlo
        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return new LoginResponse(email, tokenString);
    }
    #endregion
}