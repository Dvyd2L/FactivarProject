using DTOs.UsersMS;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Services;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IDbService<DatosPersonale, Guid> dbDatosPersonalesService,
    IDbService<Credenciale, Guid> dbCredencialesService,
    IHashService hashService,
    TokenService tokenService
    ) : ControllerBase
{
    #region PROPs
    private readonly TokenService _tokenService = tokenService;
    private readonly IHashService _hashService = hashService;
    private readonly IDbService<DatosPersonale, Guid> _dbDatosPersonalesService = dbDatosPersonalesService;
    private readonly IDbService<Credenciale, Guid> _dbCredencialesService = dbCredencialesService;
    #endregion PROPs

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO input)
    {
        // Comprueba si la entrada es nula
        if (input is null)
            return BadRequest("Entrada no válida");

        // Lee los usuarios existentes de la base de datos
        IEnumerable<DatosPersonale>? users = await _dbDatosPersonalesService.Read();

        // Comprueba si el email ya está registrado
        string? validMail = users?.FirstOrDefault(x => x.Email == input.Email)?.Email;

        if (validMail is not null)
            return BadRequest($"El email {input.Email} ya está registrado");

        // Obtiene el hash de la contraseña
        IHashResult hashResult = _hashService.GetHash(input.Password);

        // Crea un nuevo objeto DatosPersonale
        DatosPersonale newUser = new()
        {
            // No necesitas especificar el Id aquí, EF lo hará por ti
            Email = input.Email,
            Nombre = input.Nombre,
            Apellidos = input.Apellidos,
            Telefono = input.Telefono,
            Credenciale = new()
            {
                // El IdUsuario se establecerá automáticamente al Id del newUser cuando guardes en la base de datos
                Password = hashResult.Hash,
                Salt = hashResult.Salt,
            },
        };

        // Guarda el nuevo usuario en la base de datos
        await _dbDatosPersonalesService.Create(newUser);

        // Devuelve un código de estado 201 (Created)
        return Created();
    }

    [HttpPost("/login")]
    public async Task<ActionResult<ILoginResponse>> Login([FromBody] LoginUserDTO input)
    {
        if (input is null)
            return BadRequest("Entrada no válida");

        IEnumerable<DatosPersonale>? users = await _dbDatosPersonalesService.Read();
        DatosPersonale? userDB = users?.FirstOrDefault(x => x.Email == input.Email);

        if (userDB is null)
            return BadRequest($"El email {input.Email} no está registrado");

        Credenciale? credencialesDB = await _dbCredencialesService.Read(userDB.Id);

        if (credencialesDB is null)
            return BadRequest($"El usuario {input.Email} no tiene credenciales");

        IHashResult hashResult = _hashService.GetHash(input.Password, credencialesDB.Salt);

        bool validPassword = hashResult.Hash == credencialesDB.Password;

        if (!validPassword)
            return BadRequest("La contraseña no es correcta");

        ILoginResponse response = _tokenService.GenerarToken(userDB.Email, userDB.Id.ToString());

        return Ok(response);
    }
}
