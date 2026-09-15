using Distags.Core.Entities;
using System.Linq.Expressions;

namespace Distags.Core.Sorting.Levels;

public class LevelUpdatedOnSorter : PropertyQueryableSorter<Level>
{
#nullable disable
    private static readonly Expression<Func<Level, object>> UpdatedOnSelector =
        level => level is WorkshopLevel ? ((WorkshopLevel)level).UpdatedOn : default;
#nullable enable

    public LevelUpdatedOnSorter(SortOrder sortOrder) : base(UpdatedOnSelector, sortOrder)
    {
    }
}
