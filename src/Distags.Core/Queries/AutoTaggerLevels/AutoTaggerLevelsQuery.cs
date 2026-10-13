using Distags.Contracts;
using Distags.Core.Filtering.Levels;

namespace Distags.Core.Queries.AutoTaggerLevels;

public class AutoTaggerLevelsQuery : IQuery<List<LevelDto>>
{
    public LevelFilterParameters FilterParameters { get; set; } = new LevelFilterParameters();

    public LevelSortStrategy SortStrategy { get; set; }

    public Cursor? Cursor { get; set; }
}
