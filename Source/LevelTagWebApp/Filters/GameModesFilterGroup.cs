using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class GameModesFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Sprint", Tag = "sprint" },
        new TagFilter { Name = "Challenge", Tag = "challenge" },
        new TagFilter { Name = "Stunt", Tag = "stunt" },
        new TagFilter { Name = "Reverse Tag", Tag = "reverse-tag" },
        new TagFilter { Name = "Main Menu", Tag = "main-menu" },
    ];

    public override string Name => "Game Modes";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}