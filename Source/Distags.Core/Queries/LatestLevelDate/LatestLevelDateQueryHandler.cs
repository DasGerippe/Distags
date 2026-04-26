using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LatestLevelDate;

public class LatestLevelDateQueryHandler : IQueryHandler<LatestLevelDateQuery, DateTimeOffset?>
{
    private readonly DbContext _DbContext;

    public LatestLevelDateQueryHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<DateTimeOffset?> HandleAsync(LatestLevelDateQuery query)
    {
        DateTimeOffset? latestCreatedDate = await _DbContext.Set<Level>()
            .MaxAsync(level => (DateTimeOffset?)level.CreatedOn)
            .ConfigureAwait(false);

        DateTimeOffset? latestUpdatedDate = await _DbContext.Set<Level>()
            .MaxAsync(level => (DateTimeOffset?)level.UpdatedOn)
            .ConfigureAwait(false);

        if (latestCreatedDate == null)
            return latestUpdatedDate;

        if (latestUpdatedDate == null)
            return latestCreatedDate;

        return latestCreatedDate > latestUpdatedDate ? latestCreatedDate : latestUpdatedDate;
    }
}