using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi.Sorting;

public class LevelNameSorter : DefaultQueryableSorter<Level>
{
    public LevelNameSorter() : base(level => level.Name)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source, SortOrder sortOrder)
    {
        // Duplicate level names exist, so we additionally sort by level ID.
        return base.Sort(source, sortOrder).ThenByDescending(level => level.Id);
    }
}