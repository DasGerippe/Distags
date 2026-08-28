using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LatestWorkshopLevelDate;

public class LatestWorkshopLevelDateQueryHandler : IQueryHandler<LatestWorkshopLevelDateQuery, DateTimeOffset?>
{
    private readonly DistagsDbContext _DbContext;

    public LatestWorkshopLevelDateQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<DateTimeOffset?> HandleAsync(LatestWorkshopLevelDateQuery query)
    {
        DateTimeOffset? latestCreatedDate = await _DbContext.Set<WorkshopLevel>()
            .MaxAsync(level => (DateTimeOffset?)level.CreatedOn)
            .ConfigureAwait(false);

        DateTimeOffset? latestUpdatedDate = await _DbContext.Set<WorkshopLevel>()
            .MaxAsync(level => (DateTimeOffset?)level.UpdatedOn)
            .ConfigureAwait(false);

        if (latestCreatedDate == null)
            return latestUpdatedDate;

        if (latestUpdatedDate == null)
            return latestCreatedDate;

        return latestCreatedDate > latestUpdatedDate ? latestCreatedDate : latestUpdatedDate;
    }
}
