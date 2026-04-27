using DataClasses;

namespace Distags.Core.Sorting.Levels;

public class LevelCreatorSorter : PropertyQueryableSorter<Level>
{
    public LevelCreatorSorter(SortOrder sortOrder) : base(level => level.Creator!.Name, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        // Creators can have multiple levels, so we additionally sort by level ID (newer levels first).
        return base.Sort(source).ThenByDescending(level => level.Id);
    }
}