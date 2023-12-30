using DTOs.UsersMS;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
    IDbService<Usuario, Guid> dbService,
    IHashService hashService,
    TokenService tokenService
    ) : ControllerBase
{
    #region PROPs
    private readonly TokenService _tokenService = tokenService;
    private readonly IHashService _hashService = hashService;
    private readonly IDbService<Usuario, Guid> _dbService = dbService;
    #endregion PROPs

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
        => Ok(await _dbService.Read());

    [Authorize]
    [HttpGet("{pk:guid}")]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get([FromRoute] Guid pk)
        => Ok(await _dbService.Read(pk));

    [HttpPost("/register")]
    public async Task<IActionResult> Create([FromBody] UserDTO input)
    {
        if (input is null)
            return BadRequest("Entrada no válida");

        IEnumerable<Usuario>? users = await _dbService.Read();
        string? validMail = users?.FirstOrDefault(x => x.Email == input.Email)?.Email;

        if (validMail is not null)
            return BadRequest($"El email {input.Email} ya está registrado");

        IHashResult hashResult = _hashService.GetHash(input.Password);

        Usuario newUser = new()
        {
            Email = input.Email,
            Password = hashResult.Hash,
            Salt = hashResult.Salt,
        };

        await _dbService.Create(newUser);

        return Created();
    }

    [HttpPost("/login")]
    public async Task<ActionResult<ILoginResponse>> Login([FromBody] UserDTO input)
    {
        if (input is null)
            return BadRequest("Entrada no válida");

        IEnumerable<Usuario>? users = await _dbService.Read();
        Usuario? userDB = users?.FirstOrDefault(x => x.Email == input.Email);

        if (userDB is null)
            return BadRequest($"El email {input.Email} no está registrado");

        IHashResult hashResult = _hashService.GetHash(input.Password, userDB.Salt);

        bool validPassword = hashResult.Hash == userDB.Password;

        if (!validPassword)
            return BadRequest("La contraseña no es correcta");

        ILoginResponse response = _tokenService.GenerarToken(userDB.Email, userDB.Id.ToString());

        return Ok(response);
    }
}
