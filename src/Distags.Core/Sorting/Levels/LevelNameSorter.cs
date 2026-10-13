using Distags.Core.Entities;

namespace Distags.Core.Sorting.Levels;

public class LevelNameSorter : PropertyQueryableSorter<Level, string>
{
    public LevelNameSorter(SortOrder sortOrder)
        : base(level => level.Name, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        // Duplicate level names exist, so we additionally sort by level ID (newer levels first).
        return base.Sort(source).ThenByDescending(level => level.Id);
    }
}
