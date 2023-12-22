using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class DbService<TModel, TPrimaryKey>(DbContext context)
    : IDbService<TModel, TPrimaryKey>
    where TModel : class
{
    #region PROPs
    private readonly DbContext _context = context;
    private readonly DbSet<TModel> _dbTable = context.Set<TModel>();
    #endregion PROPs

    #region METHODs
    public async Task<IEnumerable<TModel>>? GetFromDB()
    {
        IEnumerable<TModel>? result = await _dbTable.ToArrayAsync();

        return result;
    }

    public async Task<TModel?> GetFromDB(TPrimaryKey pk)
    {
        TModel? result = await _dbTable.FindAsync(pk);

        return result;
    }

    public async Task PostToBD(TModel data)
    {
        _ = await _dbTable.AddAsync(data);
        _ = await _context.SaveChangesAsync();
    }
    #endregion METHODs
}
