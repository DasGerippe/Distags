using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Projection;
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
        WorkshopLevelDto? levelDto = await _DbContext.Set<WorkshopLevel>()
            .Where(level => level.WorkshopId == query.WorkshopId)
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto ?? throw new UnknownLevelException();
    }
}
