using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.TagTree;

public class TagTreeQueryHandler : IQueryHandler<TagTreeQuery, List<TagGroupDto>>
{
    private readonly DistagsDbContext _DbContext;

    public TagTreeQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<TagGroupDto>> HandleAsync(TagTreeQuery query)
    {
        return await _DbContext.Set<TagGroup>()
            .OrderBy(tagGroup => tagGroup.Position)
            .Select(new TagGroupMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}