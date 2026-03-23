using AutoTagging.Abstractions;
using DataClasses;

namespace AutoTaggerRunner;

public class AutoTaggerStateUpdater : IAutoTaggerStateUpdater
{
    public void UpdateAutoTaggerState(AutoTaggerState state, Level level, TaggingStrategy taggingStrategy)
    {
        state.LastAutoTagging = taggingStrategy switch
        {
            TaggingStrategy.NewLevels => level.CreatedOn,
            TaggingStrategy.NewAndUpdatedLevels => level.UpdatedOn,
            _ => throw new ArgumentOutOfRangeException(nameof(taggingStrategy), taggingStrategy, null),
        };
    }
}