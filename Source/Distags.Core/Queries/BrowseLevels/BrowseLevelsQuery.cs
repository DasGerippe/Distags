using Distags.Contracts;
using Distags.Core.Filtering.Levels;
using Distags.Core.Sorting.Levels;

namespace Distags.Core.Queries.BrowseLevels;

public class BrowseLevelsQuery : IQuery<LevelsPage>
{
    public LevelFilterParameters FilterParameters { get; set; } = new LevelFilterParameters();

    public LevelSortStrategy SortStrategy { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
