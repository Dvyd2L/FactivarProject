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

    private static Expression<Func<TModel, bool>> BuildEqualsExpression<TField>(Expression<Func<TModel, TField>> fieldSelector, TField value)
    {
        BinaryExpression equalExpression = Expression.Equal(fieldSelector.Body, Expression.Constant(value));
        return Expression.Lambda<Func<TModel, bool>>(equalExpression, fieldSelector.Parameters);
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

    public async Task<IEnumerable<TModel>?> Read(bool tracking = false, params Expression<Func<TModel, object>>[] include)
    {
        IQueryable<TModel> query = tracking
            ? _dbTable.AsTracking().AsQueryable()
            : _dbTable.AsQueryable();

        foreach (Expression<Func<TModel, object>> includeProperty in include)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToArrayAsync();
    }

    public async Task<TModel?> Read(TPrimaryKey pk, bool tracking = false, params Expression<Func<TModel, object>>[] include)
    {
        IQueryable<TModel> query = tracking
            ? _dbTable.AsTracking().AsQueryable()
            : _dbTable.AsQueryable();

        foreach (Expression<Func<TModel, object>> includeProperty in include)
        {
            query = query.Include(includeProperty);
        }

        PropertyInfo keyProperty = GetPrimaryKeyInfo<TModel>();
        ParameterExpression parameter = Expression.Parameter(typeof(TModel), "e");
        Expression<Func<TModel, bool>> condition = Expression.Lambda<Func<TModel, bool>>(
            Expression.Equal(
                Expression.Property(parameter, keyProperty.Name),
                Expression.Constant(pk)),
            parameter);

        return await query.SingleOrDefaultAsync(condition);
    }

    public async Task<TModel?> Read<TField>(Expression<Func<TModel, TField>> fieldSelector, TField value, bool tracking = false)
    {
        TModel? result = tracking
            ? await _dbTable.AsTracking().SingleOrDefaultAsync(BuildEqualsExpression(fieldSelector, value))
            : await _dbTable.SingleOrDefaultAsync(BuildEqualsExpression(fieldSelector, value));

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
