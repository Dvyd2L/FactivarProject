using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Handlers;

public class BasicController<TModel, TPrimaryKey>(
    [FromServices] IDbService<TModel, TPrimaryKey> dbService
    )
    : ControllerBase,
    IBasicController<TModel, TPrimaryKey>
    where TModel : class
{
    #region PROPs
    private readonly IDbService<TModel, TPrimaryKey> _service = dbService;
    #endregion PROPs

    #region METHODs
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TModel>>> Get()
    {
        IEnumerable<TModel>? result = await _service.GetFromDB();

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{pk}")]
    public virtual async Task<ActionResult<TModel>> GetByPK([FromRoute] TPrimaryKey pk)
    {
        TModel? result = await _service.GetFromDB(pk);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(object data)
    {
        if (data is null)
            return BadRequest();

        else if (data is not TModel)
            return BadRequest();

        else
            await _service.PostToBD(data as TModel);

        return NoContent();
    }

    //[HttpDelete]
    //public virtual async Task<IActionResult> Delete(object data)
    //{
    //    if (data is null)
    //        return BadRequest();

    //    else if (data is not TModel)
    //        return BadRequest();

    //    else
    //        await _service.DeleteFromBD(data as TModel);

    //    return NoContent();
    //}
    #endregion METHODs
}