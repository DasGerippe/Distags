using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class MiscFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Medals", Tag = "medals" },
        new TagFilter { Name = "Missing Level Bug", Tag = "missing-level" },
    ];

    public override string Name => "Miscellaneous";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}