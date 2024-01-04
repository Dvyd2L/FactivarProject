using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;

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
    public async Task<ActionResult<IEnumerable<DatosPersonale>>> Get()
    {
        IEnumerable<DatosPersonale>? result = await _dbService.Read();

        return result is null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("{pk:guid}")]
    public async Task<ActionResult<DatosPersonale>> Get([FromRoute] Guid pk)
    {
        DatosPersonale? result = await _dbService.Read(pk);

        return result is null
            ? NotFound()
            : Ok(result);
    }
}
