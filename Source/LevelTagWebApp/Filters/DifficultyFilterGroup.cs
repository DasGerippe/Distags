using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class DifficultyFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Casual", Tag = "casual" },
        new TagFilter { Name = "Normal", Tag = "normal" },
        new TagFilter { Name = "Advanced", Tag = "advanced" },
        new TagFilter { Name = "Expert", Tag = "expert" },
        new TagFilter { Name = "Nightmare", Tag = "nightmare" },
        new TagFilter { Name = "Brutal", Tag = "brutal", State = TagFilterState.Forbidden },
        new TagFilter { Name = "Lunatic", Tag = "lunatic", State = TagFilterState.Forbidden },
        new TagFilter { Name = "Insane", Tag = "insane", State = TagFilterState.Forbidden },
        new TagFilter { Name = "Impossible", Tag = "impossible", State = TagFilterState.Forbidden },
    ];

    public override string Name => "Difficulty";

    public override FilterGroupType FilterGroupType => FilterGroupType.ExclusiveTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}