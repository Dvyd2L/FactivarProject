using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class DbService<TContext, TModel, TPrimaryKey>(TContext context)
    : IDbService<TModel, TPrimaryKey>
    where TContext : DbContext
    where TModel : class
{
    #region PROPs
    private readonly TContext _context = context;
    private readonly DbSet<TModel> _dbTable = context.Set<TModel>();
    #endregion PROPs

    #region METHODs

    #region GET
    public async Task<IEnumerable<TModel>?> Read()
    {
        IEnumerable<TModel>? result = await _dbTable.ToArrayAsync();

        return result;
    }

    public async Task<TModel?> Read(TPrimaryKey pk)
    {
        TModel? result = await _dbTable.FindAsync(pk);

        return result;
    }
    #endregion GET

    #region POST
    public async Task Create(TModel data)
    {
        _ = await _dbTable.AddAsync(data);
        _ = await _context.SaveChangesAsync();
    }

    public async Task Create(IEnumerable<TModel> data)
    {
        await _dbTable.AddRangeAsync(data);
        _ = await _context.SaveChangesAsync();
    }
    #endregion POST

    #region PUT
    public Task Update(TModel data) => throw new NotImplementedException();
    public Task Update(IEnumerable<TModel> data) => throw new NotImplementedException();
    #endregion PUT

    #region DELETE
    public Task Delete(TPrimaryKey pk) => throw new NotImplementedException();
    public Task Delete(IEnumerable<TPrimaryKey> pk) => throw new NotImplementedException();
    #endregion DELETE

    #endregion METHODs
}
