using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Filtering;
using Distags.Core.Filtering.Levels;
using Distags.Core.Projection;
using Distags.Core.Sorting;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.AutoTaggerLevels;

public class AutoTaggerLevelsQueryHandler : IQueryHandler<AutoTaggerLevelsQuery, List<LevelDto>>
{
    private readonly DistagsDbContext _DbContext;
    private readonly IQueryableFilterFactory<Level, LevelFilterParameters> _LevelFilterFactory;
    private readonly IQueryableCursorFilterFactory<Level, LevelSortStrategy> _LevelCursorFilterFactory;
    private readonly IQueryableSorterFactory<Level, LevelSortStrategy> _LevelSorterFactory;

    public AutoTaggerLevelsQueryHandler(
        DistagsDbContext dbContext,
        IQueryableFilterFactory<Level, LevelFilterParameters> levelFilterFactory,
        IQueryableCursorFilterFactory<Level, LevelSortStrategy> levelCursorFilterFactory,
        IQueryableSorterFactory<Level, LevelSortStrategy> levelSorterFactory)
    {
        _DbContext = dbContext;
        _LevelFilterFactory = levelFilterFactory;
        _LevelCursorFilterFactory = levelCursorFilterFactory;
        _LevelSorterFactory = levelSorterFactory;
    }

    public async Task<List<LevelDto>> HandleAsync(AutoTaggerLevelsQuery query)
    {
        IQueryable<Level> levelQuery = _DbContext.Set<Level>();

        IQueryableFilter<Level> levelFilter = _LevelFilterFactory.GetQueryableFilter(query.FilterParameters);
        levelQuery = levelFilter.Filter(levelQuery);

        if (query.Cursor != null)
            levelQuery = ApplyCursorFilter(levelQuery, query.SortStrategy, query.Cursor);

        IQueryableSorter<Level> levelSorter = _LevelSorterFactory.GetQueryableSorter(query.SortStrategy);
        levelQuery = levelSorter.Sort(levelQuery);

        List<LevelDto> levelDtos = await levelQuery
            .SelectDto()
            .ToListAsync().ConfigureAwait(false);

        return levelDtos;
    }

    private IQueryable<Level> ApplyCursorFilter(IQueryable<Level> levelQuery, LevelSortStrategy sortStrategy, Cursor cursor)
    {
        IQueryableFilter<Level> levelCursorFilter =
            _LevelCursorFilterFactory.Create(sortStrategy, ValueTuple.Create(cursor.Value, cursor.EntityId));

        return levelCursorFilter.Filter(levelQuery);
    }
}
