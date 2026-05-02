using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.StalestCreators;

public class StalestCreatorsQueryHandler : IQueryHandler<StalestCreatorsQuery, List<long>>
{
    private readonly DistagsDbContext _DbContext;

    public StalestCreatorsQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public Task<List<long>> HandleAsync(StalestCreatorsQuery query)
    {
        IQueryable<Creator> queryable = _DbContext.Set<Creator>()
            .OrderBy(creator => creator.CachedOn);

        if (query.Limit != null)
            queryable = queryable.Take(query.Limit.Value);

        return queryable.Select(creator => creator.Id).ToListAsync();
    }
}