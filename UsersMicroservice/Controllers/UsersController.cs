using DTOs.UsersMS;
using Handlers;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IDbService<Usuario, Guid> dbService)
    : BasicController<Usuario, Guid>(dbService)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDTO input)
    {
        if (input is null)
            return BadRequest("Entrada no válida");

        //input.Password = PasswordHandler.HashPassword(input.Password);

        IEnumerable<Usuario>? users = await dbService.GetFromDB();
        string? validMail = users?.FirstOrDefault(x => x.Email == input.Email)?.Email;

        if (validMail is not null)
            return BadRequest($"El email {input.Email} ya está registrado");

        Usuario newUser = new()
        {
            Email = input.Email,
            Password = input.Password,
        };

        return await base.Post(newUser);
    }
}
