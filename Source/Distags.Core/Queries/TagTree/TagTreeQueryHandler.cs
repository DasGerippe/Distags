using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.TagTree;

public class TagTreeQueryHandler : IQueryHandler<TagTreeQuery, List<TagGroupDto>>
{
    private readonly DbContext _DbContext;
    private readonly IEntityProjector<TagGroup, TagGroupDto> _TagGroupProjector;

    public TagTreeQueryHandler(DbContext dbContext, IEntityProjector<TagGroup, TagGroupDto> tagGroupProjector)
    {
        _DbContext = dbContext;
        _TagGroupProjector = tagGroupProjector;
    }

    public async Task<List<TagGroupDto>> HandleAsync(TagTreeQuery query)
    {
        return await _DbContext.Set<TagGroup>()
            .OrderBy(tagGroup => tagGroup.Position)
            .Select(_TagGroupProjector.Projection)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}