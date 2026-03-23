using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class GimmicksFilterGroup : FilterGroup
{
    private readonly TagFilter[] _TagFilters =
    [
        new TagFilter { Name = "Laps", Tag = "laps" },
        new TagFilter { Name = "Boosters", Tag = "boosters" },
        new TagFilter { Name = "Force Zones", Tag = "force-zones" },
        new TagFilter { Name = "Required Checkpoints", Tag = "required-checkpoints" },
        new TagFilter { Name = "Collectibles", Tag = "collectibles" },
        new TagFilter { Name = "Discoverables", Tag = "discoverables" },
        new TagFilter { Name = "Forced Camera", Tag = "forced-camera" },
    ];
    
    public override string Name => "Gimmicks";

    public override FilterGroupType FilterGroupType => FilterGroupType.CombinableTags;
    
    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);
}