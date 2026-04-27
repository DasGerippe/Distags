using DataClasses;
using Distags.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LevelTags;

public class LevelTagsQueryHandler : IQueryHandler<LevelTagsQuery, List<string>>
{
    private readonly DbContext _DbContext;

    public LevelTagsQueryHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<List<string>> HandleAsync(LevelTagsQuery query)
    {
        bool levelExists = await _DbContext.Set<Level>().AnyAsync(level => level.Id == query.LevelId).ConfigureAwait(false);
        if (!levelExists)
            throw new UnknownLevelException(query.LevelId);

        List<string> tags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.LevelId == query.LevelId)
            .Select(levelTag => levelTag.Tag.Name)
            .ToListAsync()
            .ConfigureAwait(false);

        return tags;
    }
}