using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class ThemeFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Distance", Tag = "distance" },
        new TagFilter { Name = "Halloween", Tag = "halloween" },
        new TagFilter { Name = "Christmas", Tag = "christmas" },
        new TagFilter { Name = "Plants", Tag = "plants" },
        new TagFilter { Name = "Desert", Tag = "desert" },
        new TagFilter { Name = "Lava", Tag = "lava" },
        new TagFilter { Name = "Water", Tag = "water" },
        new TagFilter { Name = "Sky", Tag = "sky" },
        new TagFilter { Name = "Arctic", Tag = "arctic" },
    ];

    public override string Name => "Theme";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}