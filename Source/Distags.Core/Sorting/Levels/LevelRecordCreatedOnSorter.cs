using Distags.Core.Entities;

namespace Distags.Core.Sorting.Levels;

public class LevelRecordCreatedOnSorter : PropertyQueryableSorter<Level, DateTimeOffset>
{
    public LevelRecordCreatedOnSorter(SortOrder sortOrder)
        : base(level => level.RecordCreatedOn, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        return base.Sort(source).ThenBy(level => level.Id);
    }
}
