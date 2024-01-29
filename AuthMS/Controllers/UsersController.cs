using AuthMS.Models;
using DTOs.UsersMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AuthMS.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(
    IDbService<DatosPersonale, Guid> dbService
    ) : ControllerBase
{
    #region PROPs
    private readonly IDbService<DatosPersonale, Guid> _dbService = dbService;
    #endregion PROPs

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
    {
        IEnumerable<DatosPersonale>? result = await _dbService.Read(include: x => x.Credenciale.Roles_IdRolNavigation);


        if (result == null)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }

        IEnumerable<UserDTO> response = result.Select(x => new UserDTO()
        {
            Id = x.Id.ToString(),
            Apellidos = x.Apellidos,
            AvatarUrl = x.AvatarUrl,
            Email = x.Email,
            Nombre = x.Nombre,
            Telefono = x.Telefono,
            IsAdmin = x.Credenciale?.Roles_IdRolNavigation.IdRol == 1,
        });

        return Ok(response);
    }

    [HttpGet("{pk:guid}")]
    public async Task<ActionResult<UserDTO>> Get([FromRoute] Guid pk)
    {
        DatosPersonale? userDB = await _dbService.Read(pk, include: x => x.Credenciale.Roles_IdRolNavigation);

        if (userDB == null)
        {
            return NotFound(new { Message = "Usuario no encontrado" });
        }

        UserDTO result = new()
        {
            Id = userDB.Id.ToString(),
            Apellidos = userDB.Apellidos,
            AvatarUrl = userDB.AvatarUrl,
            Email = userDB.Email,
            Nombre = userDB.Nombre,
            Telefono = userDB.Telefono,
            IsAdmin = userDB.Credenciale?.Roles_IdRolNavigation.IdRol == 1,
        };

        return Ok(result);
    }
}
