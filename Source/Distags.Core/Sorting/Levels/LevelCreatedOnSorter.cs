using Distags.Core.Entities;
using Distags.Core.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelCreatedOnSorter : PropertyQueryableSorter<Level, DateTimeOffset?>
{
    public LevelCreatedOnSorter(SortOrder sortOrder)
        : base(LevelExpressions.CreatedOnSelector, sortOrder)
    {
    }
}
