using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Projection;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.Tags;

public class TagsQueryHandler : IQueryHandler<TagsQuery, List<TagDto>>
{
    private readonly DistagsDbContext _DbContext;

    public TagsQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<TagDto>> HandleAsync(TagsQuery query)
    {
        return await _DbContext.Set<Tag>()
            .OrderBy(tag => tag.DisplayGroup.Position)
            .ThenBy(tag => tag.Position)
            .SelectDto()
            .ToListAsync()
            .ConfigureAwait(false);
    }
}
