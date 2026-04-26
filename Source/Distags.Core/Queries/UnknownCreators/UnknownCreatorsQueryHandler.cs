using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UnknownCreators;

public class UnknownCreatorsQueryHandler : IQueryHandler<UnknownCreatorsQuery, List<long>>
{
    private readonly DbContext _DbContext;

    public UnknownCreatorsQueryHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public Task<List<long>> HandleAsync(UnknownCreatorsQuery query)
    {
        return _DbContext.Set<Level>()
            .Where(level => level.Creator == null)
            .Select(level => level.CreatorId)
            .Distinct()
            .ToListAsync();
    }
}