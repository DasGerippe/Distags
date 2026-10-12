using Distags.Core.Entities;
using Distags.Core.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelCreatorSorter : PropertyQueryableSorter<Level, string>
{
    public LevelCreatorSorter(SortOrder sortOrder)
        : base(LevelExpressions.CreatorNameSelector, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        // Creators can have multiple levels, so we additionally sort by level ID (newer levels first).
        return base.Sort(source).ThenByDescending(level => level.Id);
    }
}
