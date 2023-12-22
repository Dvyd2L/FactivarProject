using Microsoft.AspNetCore.Mvc;

namespace Interfaces;
public interface IGetRequest<TModel, TPrimaryKey>
    where TModel : class
{
    Task<ActionResult<IEnumerable<TModel>>> Get();
    Task<ActionResult<TModel>> GetByPK(TPrimaryKey pk);
}