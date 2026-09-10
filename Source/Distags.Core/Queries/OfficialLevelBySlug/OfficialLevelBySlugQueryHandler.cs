using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.OfficialLevelBySlug;

public class OfficialLevelBySlugQueryHandler : IQueryHandler<OfficialLevelBySlugQuery, OfficialLevelDto>
{
    private readonly DistagsDbContext _DbContext;

    public OfficialLevelBySlugQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<OfficialLevelDto> HandleAsync(OfficialLevelBySlugQuery query)
    {
        LevelMapper levelProjector = new LevelMapper();
        LevelDto? levelDto = await _DbContext.Set<OfficialLevel>()
            .Where(level => level.Slug == query.Slug)
            .Select(levelProjector.Expression)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto as OfficialLevelDto ?? throw new UnknownLevelException();
    }
}
