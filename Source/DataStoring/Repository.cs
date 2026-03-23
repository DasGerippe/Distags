using DataStoring.Contract;
using Microsoft.EntityFrameworkCore;

namespace DataStoring;

public class Repository<TEntity, TDbContext> : IRepository<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    private readonly TDbContext _DbContext;

    public Repository(TDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public IQueryable<TEntity> Query()
    {
        return _DbContext.Set<TEntity>();
    }

    public async Task<TEntity?> FindAsync(params object?[]? keyValues)
    {
        return await _DbContext.Set<TEntity>().FindAsync(keyValues);
    }

    public async Task<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
    {
        return await _DbContext.Set<TEntity>().FindAsync(keyValues, cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _DbContext.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _DbContext.Set<TEntity>().Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _DbContext.Set<TEntity>().Remove(entity);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _DbContext.SaveChangesAsync(cancellationToken);
    }
}