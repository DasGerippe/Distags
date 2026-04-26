using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.CreatorCount;

public class CreatorCountQueryHandler : IQueryHandler<CreatorCountQuery, long>
{
    private readonly DbContext _DbContext;

    public CreatorCountQueryHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public Task<long> HandleAsync(CreatorCountQuery query)
    {
        return _DbContext.Set<Creator>().LongCountAsync();
    }
}