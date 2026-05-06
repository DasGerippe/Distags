using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.CreatorCount;

public class CreatorCountQueryHandler : IQueryHandler<CreatorCountQuery, long>
{
    private readonly DistagsDbContext _DbContext;

    public CreatorCountQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public Task<long> HandleAsync(CreatorCountQuery query)
    {
        return _DbContext.Set<Creator>().LongCountAsync();
    }
}