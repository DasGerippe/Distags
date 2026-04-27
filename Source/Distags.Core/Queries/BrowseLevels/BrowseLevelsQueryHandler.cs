using DataClasses;
using Distags.Core.Dtos;
using Distags.Core.Filtering;
using Distags.Core.Filtering.Levels;
using Distags.Core.Projection;
using Distags.Core.Sorting;
using Distags.Core.Sorting.Levels;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.BrowseLevels;

public class BrowseLevelsQueryHandler : IQueryHandler<BrowseLevelsQuery, LevelsPage>
{
    private readonly DbContext _DbContext;
    private readonly IQueryableFilterFactory<Level, LevelFilterParameters> _LevelFilterFactory;
    private readonly IQueryableSorterFactory<Level, LevelSortStrategy> _LevelSorterFactory;

    public BrowseLevelsQueryHandler(
        DbContext dbContext,
        IQueryableFilterFactory<Level, LevelFilterParameters> levelFilterFactory,
        IQueryableSorterFactory<Level, LevelSortStrategy> levelSorterFactory)
    {
        _DbContext = dbContext;
        _LevelFilterFactory = levelFilterFactory;
        _LevelSorterFactory = levelSorterFactory;
    }

    public async Task<LevelsPage> HandleAsync(BrowseLevelsQuery query)
    {
        LevelsPage levelsPage = new LevelsPage
        {
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
        };

        IQueryable<Level> levels = _DbContext.Set<Level>();

        IQueryableFilter<Level> levelFilter = _LevelFilterFactory.GetQueryableFilter(query.FilterParameters);
        levels = levelFilter.Filter(levels);

        levelsPage.TotalLevels = await levels.CountAsync().ConfigureAwait(false);
        if (levelsPage.TotalLevels == 0)
            return levelsPage;

        levelsPage.TotalPages = (int)Math.Ceiling((double)levelsPage.TotalLevels / query.PageSize);
        if (query.PageNumber > levelsPage.TotalPages)
            return levelsPage;

        IQueryableSorter<Level> levelSorter = _LevelSorterFactory.GetQueryableSorter(query.SortStrategy);
        levels = levelSorter.Sort(levels);

        if (query.PageNumber > 1)
            levels = levels.Skip((query.PageNumber - 1) * query.PageSize);

        if (levelsPage.TotalLevels - query.PageNumber * query.PageSize > query.PageSize)
            levels = levels.Take(query.PageSize);

        LevelProjector levelProjector = new LevelProjector();
        List<LevelDto> levelDtos = await levels
            .Select(levelProjector.Projection)
            .ToListAsync()
            .ConfigureAwait(false);

        levelsPage.Levels = levelDtos;

        return levelsPage;
    }
}