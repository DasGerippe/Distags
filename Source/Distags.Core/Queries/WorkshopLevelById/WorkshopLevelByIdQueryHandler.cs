using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.WorkshopLevelById;

public class WorkshopLevelByIdQueryHandler : IQueryHandler<WorkshopLevelByIdQuery, WorkshopLevelDto>
{
    private readonly DistagsDbContext _DbContext;

    public WorkshopLevelByIdQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<WorkshopLevelDto> HandleAsync(WorkshopLevelByIdQuery query)
    {
        LevelMapper levelProjector = new LevelMapper();
        LevelDto? levelDto = await _DbContext.Set<WorkshopLevel>()
            .Where(level => level.WorkshopId == query.WorkshopId)
            .Select(levelProjector.Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto as WorkshopLevelDto ?? throw new UnknownLevelException();
    }
}
