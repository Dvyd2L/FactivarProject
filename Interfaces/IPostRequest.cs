using Microsoft.AspNetCore.Mvc;

namespace Interfaces;

public interface IPostRequest<TModel>
    where TModel : class
{
    Task<IActionResult> Post(object data);
}
