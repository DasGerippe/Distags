using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LevelById;

public class LevelByIdQueryHandler : IQueryHandler<LevelByIdQuery, LevelDto>
{
    private readonly DistagsDbContext _DbContext;

    public LevelByIdQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<LevelDto> HandleAsync(LevelByIdQuery query)
    {
        LevelMapper levelProjector = new LevelMapper();
        LevelDto? levelDto = await _DbContext.Set<Level>()
            .Where(level => level.Id == query.LevelId)
            .Select(levelProjector.Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto ?? throw new UnknownLevelException(query.LevelId);
    }
}
