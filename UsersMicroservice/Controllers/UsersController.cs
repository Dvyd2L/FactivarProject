using DTOs.UsersMS;
using Handlers;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IDbService<Usuario, Guid> dbService, IHashService hashService)
    : BasicController<Usuario, Guid>(dbService)
{
    #region MyRegion
    private readonly IHashService _hashService = hashService;
    private readonly IDbService<Usuario, Guid> _dbService = dbService;
    #endregion

    [HttpPost("/register")]
    public async Task<IActionResult> Create([FromBody] UserDTO input)
    {
        if (input is null)
            return BadRequest("Entrada no válida");

        //input.Password = PasswordHandler.HashPassword(input.Password);

        IEnumerable<Usuario>? users = await _dbService.GetFromDB();
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

        return await base.Post(newUser);
    }
}
