using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelCreatedOnSorter : PropertyQueryableSorter<Level>
{
#nullable disable
    private static readonly Expression<Func<Level, object>> CreatedOnSelector =
        level => level is WorkshopLevel ? ((WorkshopLevel)level).CreatedOn : default;
#nullable enable

    public LevelCreatedOnSorter(SortOrder sortOrder) : base(CreatedOnSelector, sortOrder)
    {
    }
}
