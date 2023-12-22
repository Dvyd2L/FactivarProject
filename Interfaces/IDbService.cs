namespace Interfaces;
public interface IDbService<TModel, TPrimaryKey>
    where TModel : class
{
    Task<IEnumerable<TModel>>? GetFromDB();
    Task<TModel?> GetFromDB(TPrimaryKey pk);
    Task PostToBD(TModel data);
}
