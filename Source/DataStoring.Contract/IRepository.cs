namespace DataStoring.Contract;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task SaveChangesAsync();
}