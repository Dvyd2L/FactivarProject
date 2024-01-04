using System.Linq.Expressions;

namespace Services.Interfaces;
public interface IDbService<TModel, TPrimaryKey>
    where TModel : class
{
    #region GET
    Task<IEnumerable<TModel>?> Read(bool tracking = false);
    Task<IEnumerable<TModel>?> Read(bool tracking = false, params Expression<Func<TModel, object>>[] include);
    Task<TModel?> Read(TPrimaryKey pk, bool tracking = false);
    Task<TModel?> Read(TPrimaryKey pk, bool tracking = false, params Expression<Func<TModel, object>>[] include);
    Task<TModel?> Read<TField>(Expression<Func<TModel, TField>> fieldSelector, TField value, bool tracking = false);
    #endregion GET

    #region POST
    Task Create(TModel data);
    Task Create(IEnumerable<TModel> data);
    #endregion POST

    #region PUT
    Task Update(TModel data);
    Task Update(IEnumerable<TModel> data);
    #endregion PUT

    #region DELETE
    Task Delete(TPrimaryKey pk);
    Task Delete(IEnumerable<TPrimaryKey> pk);
    #endregion DELETE
}
