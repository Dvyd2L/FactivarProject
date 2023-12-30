using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersMicroservice.Models;

namespace UsersMicroservice.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(
    IDbService<Usuario, Guid> dbService
    ) : ControllerBase
{
    #region PROPs
    private readonly IDbService<Usuario, Guid> _dbService = dbService;
    #endregion PROPs

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
    {
        IEnumerable<Usuario>? result = await _dbService.Read();

        if (result is null)
            return NotFound();

        List<object> response = [];

        foreach (Usuario item in result)
        {
            response.Add(new
            {
                item.Id,
                item.Email,
            });
        }

        return Ok(response);
    }

    [HttpGet("{pk:guid}")]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get([FromRoute] Guid pk)
    {
        Usuario? result = await _dbService.Read(pk);

        if (result is null)
            return NotFound();

        var response = new
        {
            result.Id,
            result.Email,
        };

        return Ok(response);
    }
}
