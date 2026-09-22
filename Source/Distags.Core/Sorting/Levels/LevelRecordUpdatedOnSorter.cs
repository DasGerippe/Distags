using Distags.Core.Entities;

namespace Distags.Core.Sorting.Levels;

public class LevelRecordUpdatedOnSorter : PropertyQueryableSorter<Level>
{
    public LevelRecordUpdatedOnSorter(SortOrder sortOrder) : base(level => level.RecordUpdatedOn, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        return base.Sort(source).ThenBy(level => level.Id);
    }
}
