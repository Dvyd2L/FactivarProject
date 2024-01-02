using Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

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
    private static PropertyInfo GetPrimaryKeyInfo<T>()
        where T : class
    {
        Type type = typeof(T);
        List<PropertyInfo> allProperties = type
            .GetProperties()
            .Where(prop => prop.IsDefined(typeof(KeyAttribute), false))
            .ToList();

        return allProperties.Count == 1
            ? allProperties[0]
            : throw new ApplicationException($"Cannot find primary key property for type {type.FullName}");
    }

    #region GET
    public async Task<IEnumerable<TModel>?> Read(bool tracking = false)
    {
        TModel[] result = tracking
            ? await _dbTable.AsTracking().ToArrayAsync()
            : await _dbTable.ToArrayAsync();

        return result;
    }

    public async Task<TModel?> Read(TPrimaryKey pk, bool tracking = false)
    {
        PropertyInfo keyProperty = GetPrimaryKeyInfo<TModel>();
        ParameterExpression parameter = Expression.Parameter(typeof(TModel), "e");
        Expression<Func<TModel, bool>> condition = Expression.Lambda<Func<TModel, bool>>(
            Expression.Equal(
                Expression.Property(parameter, keyProperty.Name),
                Expression.Constant(pk)),
            parameter);

        TModel? result = tracking
            ? await _dbTable.AsTracking().SingleOrDefaultAsync(condition)
            : await _dbTable.FindAsync(pk);

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
    public async Task Update(TModel data)
    {
        _ = _dbTable.Update(data);
        _ = await _context.SaveChangesAsync();
    }
    public async Task Update(IEnumerable<TModel> data)
    {
        _dbTable.UpdateRange(data);
        _ = await _context.SaveChangesAsync();
    }
    #endregion PUT

    #region DELETE
    public async Task Delete(TPrimaryKey pk)
    {
        TModel? data = await _dbTable.FindAsync(pk);
        if (data is not null)
        {
            _ = _dbTable.Remove(data);
            _ = await _context.SaveChangesAsync();
        }
    }
    public async Task Delete(IEnumerable<TPrimaryKey> pk)
    {
        TModel? data = await _dbTable.FindAsync(pk);
        if (data is not null)
        {
            _dbTable.RemoveRange(data);
            _ = await _context.SaveChangesAsync();
        }
    }
    #endregion DELETE

    #endregion METHODs
}
