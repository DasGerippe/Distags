using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Exceptions;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LevelById;

public class LevelByIdQueryHandler : IQueryHandler<LevelByIdQuery, LevelDto>
{
    private readonly DbContext _DbContext;

    public LevelByIdQueryHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<LevelDto> HandleAsync(LevelByIdQuery query)
    {
        LevelProjector levelProjector = new LevelProjector();
        LevelDto? levelDto = await _DbContext.Set<Level>()
            .Where(level => level.Id == query.LevelId)
            .Select(levelProjector.Projection)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto ?? throw new UnknownLevelException(query.LevelId);
    }
}