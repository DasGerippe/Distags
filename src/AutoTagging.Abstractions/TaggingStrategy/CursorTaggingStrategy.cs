using Distags.Contracts;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class CursorTaggingStrategy
{
    private readonly LevelType? _LevelType;
    private readonly LevelSortStrategy _SortStrategy;
    private readonly Func<LevelDto, DateTimeOffset> _CursorTimestampSelector;

    public CursorTaggingStrategy(
        Func<LevelDto, DateTimeOffset> cursorTimestampSelector,
        LevelSortStrategy sortStrategy,
        LevelType? levelType = null)
    {
        _CursorTimestampSelector = cursorTimestampSelector;
        _SortStrategy = sortStrategy;
        _LevelType = levelType;
    }

    public AutoTaggerLevelsQuery GetLevelsQuery(TimestampLevelIdCursor? cursor)
    {
        return new AutoTaggerLevelsQuery
        {
            FilterParameters = new LevelFilterParameters
            {
                Type = _LevelType,
            },
            SortStrategy = _SortStrategy,
            Cursor = cursor == null ? null : new Cursor
            {
                Value = cursor.Timestamp,
                EntityId = cursor.LevelId,
            },
        };
    }

    public TimestampLevelIdCursor UpdateCursor(TimestampLevelIdCursor? cursor, LevelDto level)
    {
        cursor ??= new TimestampLevelIdCursor();
        cursor.Timestamp = _CursorTimestampSelector(level);
        cursor.LevelId = level.Id;
        return cursor;
    }
}
