using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Mapping;
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
            .Select(new TagMapper().Expression)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}