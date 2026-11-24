using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions.Progress;

public interface IProgressStrategy
{
    AutoTaggerLevelsQuery GetLevelsQuery(TimestampLevelIdCursor? cursor);

    TimestampLevelIdCursor UpdateCursor(TimestampLevelIdCursor? cursor, LevelDto level);
}
