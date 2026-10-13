using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Exceptions;
using Distags.Core.Projection;
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
        OfficialLevelDto? levelDto = await _DbContext.Set<OfficialLevel>()
            .Where(level => level.Slug == query.Slug)
            .SelectDto()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelDto ?? throw new UnknownLevelException();
    }
}
