using Distags.Core.Entities;
using Distags.Core.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelUpdatedOnSorter : PropertyQueryableSorter<Level, DateTimeOffset?>
{
    public LevelUpdatedOnSorter(SortOrder sortOrder)
        : base(LevelExpressions.UpdatedOnSelector, sortOrder)
    {
    }
}
