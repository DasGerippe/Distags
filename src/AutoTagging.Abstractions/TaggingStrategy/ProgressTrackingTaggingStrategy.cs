using AutoTagging.Abstractions.Progress;
using Distags.Contracts;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class ProgressTrackingTaggingStrategy : ITaggingStrategy<TimestampLevelIdProgress>
{
    private readonly LevelType? _LevelType;
    private readonly LevelSortStrategy _SortStrategy;
    private readonly Func<LevelDto, DateTimeOffset> _ProgressTimestampSelector;

    public ProgressTrackingTaggingStrategy(
        Func<LevelDto, DateTimeOffset> progressTimestampSelector,
        LevelSortStrategy sortStrategy,
        LevelType? levelType = null)
    {
        _ProgressTimestampSelector = progressTimestampSelector;
        _SortStrategy = sortStrategy;
        _LevelType = levelType;
    }

    public AutoTaggerLevelsQuery GetLevelsQuery(TimestampLevelIdProgress? progress)
    {
        return new AutoTaggerLevelsQuery
        {
            FilterParameters = new LevelFilterParameters
            {
                Type = _LevelType,
            },
            SortStrategy = _SortStrategy,
            Cursor = progress == null ? null : new Cursor
            {
                Value = progress.Timestamp,
                EntityId = progress.LevelId,
            },
        };
    }

    public TimestampLevelIdProgress? UpdateProgress(TimestampLevelIdProgress? progress, LevelDto level)
    {
        progress ??= new TimestampLevelIdProgress();
        progress.Timestamp = _ProgressTimestampSelector(level);
        progress.LevelId = level.Id;
        return progress;
    }
}
