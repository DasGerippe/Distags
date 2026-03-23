using DataClasses;

namespace LevelTagApi.Sorting;

public class LevelNameSorter : DefaultQueryableSorter<Level>
{
    public LevelNameSorter(SortOrder sortOrder) : base(level => level.Name, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        // Duplicate level names exist, so we additionally sort by level ID.
        return base.Sort(source).ThenBy(level => level.Id);
    }
}