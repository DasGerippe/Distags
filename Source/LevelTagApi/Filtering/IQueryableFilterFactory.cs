namespace LevelTagApi.Filtering;

public interface IQueryableFilterFactory<TEntity, TFilterParams>
{
    IQueryableFilter<TEntity> GetQueryableFilter(TFilterParams filterParams);
}