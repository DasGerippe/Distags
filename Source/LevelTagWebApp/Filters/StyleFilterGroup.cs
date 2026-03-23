using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class StyleFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Colorful", Tag = "colorful" },
        new TagFilter { Name = "Monochromatic", Tag = "monochromatic"},
        new TagFilter { Name = "Dark", Tag = "dark"},
        new TagFilter { Name = "Bright", Tag = "bright"},
        new TagFilter { Name = "Neon/Contrast", Tag = "contrast"},
    ];

    public override string Name => "Style";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}