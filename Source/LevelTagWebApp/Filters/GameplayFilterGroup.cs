using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class GameplayFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Standard", Tag = "standard" },
        new TagFilter { Name = "Flat Driving", Tag = "flat-driving" },
        new TagFilter { Name = "Obstacles", Tag = "obstacles" },
        new TagFilter { Name = "Transfers", Tag = "transfers" },
        new TagFilter { Name = "Wing Flight", Tag = "wing-flight" },
        new TagFilter { Name = "Grip Flight", Tag = "grip-flight" },
        new TagFilter { Name = "Low Drag", Tag = "low-drag" },
        new TagFilter { Name = "Climbing", Tag = "climbing" },
        new TagFilter { Name = "Precision", Tag = "precision" },
        new TagFilter { Name = "Tech", Tag = "tech" },
        new TagFilter { Name = "Jank", Tag = "jank" },
        new TagFilter { Name = "Boat", Tag = "boat" },
        new TagFilter { Name = "Minigame", Tag = "minigame" },
        new TagFilter { Name = "Puzzle", Tag = "puzzle" },
    ];

    public override string Name => "Gameplay";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}