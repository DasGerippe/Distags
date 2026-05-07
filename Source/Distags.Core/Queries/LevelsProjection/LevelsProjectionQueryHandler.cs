using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Filtering;
using Distags.Core.Filtering.Levels;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.LevelsProjection;

public class LevelsProjectionQueryHandler : IQueryHandler<LevelsProjectionQuery, List<LevelProjectionDto>>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IQueryableFilterFactory<Level, LevelFilterParameters> _LevelFilterFactory;

    public LevelsProjectionQueryHandler(
        DistagsDbContext dbContext,
        IQueryableFilterFactory<Level, LevelFilterParameters> levelFilterFactory)
    {
        _DbContext = dbContext;
        _LevelFilterFactory = levelFilterFactory;
    }

    public async Task<List<LevelProjectionDto>> HandleAsync(LevelsProjectionQuery query)
    {
        IQueryable<Level> levels = _DbContext.Set<Level>();

        IQueryableFilter<Level> levelFilter = _LevelFilterFactory.GetQueryableFilter(query.FilterParameters);
        levels = levelFilter.Filter(levels);

        List<LevelProjectionDto> levelDtos = await levels
            .Select(new LevelProjectionMapper(query.Includes).Expression)
            .ToListAsync().ConfigureAwait(false);

        return levelDtos;
    }
}