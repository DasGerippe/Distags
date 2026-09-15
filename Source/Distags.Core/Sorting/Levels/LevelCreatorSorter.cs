using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelCreatorSorter : PropertyQueryableSorter<Level>
{
#nullable disable
    private static readonly Expression<Func<Level, object>> CreatorNameSelector = level =>
        level is OfficialLevel ? ((OfficialLevel)level).CreatorName
        : level is WorkshopLevel ? ((WorkshopLevel)level).Creator.Name
        : null;
#nullable enable

    public LevelCreatorSorter(SortOrder sortOrder) : base(CreatorNameSelector, sortOrder)
    {
    }

    public override IOrderedQueryable<Level> Sort(IQueryable<Level> source)
    {
        // Creators can have multiple levels, so we additionally sort by level ID (newer levels first).
        return base.Sort(source).ThenByDescending(level => level.Id);
    }
}
