namespace DataStoring.Contract;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();

    Task<TEntity?> FindAsync(params object?[]? keyValues);

    Task<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}