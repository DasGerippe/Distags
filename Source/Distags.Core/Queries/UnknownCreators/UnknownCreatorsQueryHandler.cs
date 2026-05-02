using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.UnknownCreators;

public class UnknownCreatorsQueryHandler : IQueryHandler<UnknownCreatorsQuery, List<long>>
{
    private readonly DistagsDbContext _DbContext;

    public UnknownCreatorsQueryHandler(DistagsDbContext dbContext)
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