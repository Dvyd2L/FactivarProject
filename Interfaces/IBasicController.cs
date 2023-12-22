namespace Interfaces;
public interface IBasicController<TModel, TPrimaryKey>
    : IGetRequest<TModel, TPrimaryKey>,
    IPostRequest<TModel>
    where TModel : class
{
}
