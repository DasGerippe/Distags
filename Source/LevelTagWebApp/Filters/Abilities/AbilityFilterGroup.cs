using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters.Abilities;

public abstract class AbilityFilterGroup : FilterGroup
{
    protected readonly TagFilter[] _TagFilters;

    public override string Name { get; }

    public override FilterGroupType FilterGroupType => FilterGroupType.ExclusiveTags;

    public override IReadOnlyList<TagFilter> TagFilters => new ReadOnlyCollection<TagFilter>(_TagFilters);

    protected AbilityFilterGroup(string abilityName, string abilityTag)
    {
        Name = abilityName;
        _TagFilters = CreateTagFilters(abilityTag);
    }

    protected TagFilter[] CreateTagFilters(string abilityTagPart)
    {
        return
        [
            new TagFilter { Name = "Enabled", Tag = $"{abilityTagPart}-enabled" },
            new TagFilter { Name = "Toggling", Tag = $"{abilityTagPart}-toggling", State = TagFilterState.Forbidden },
            new TagFilter { Name = "Disabled", Tag = $"{abilityTagPart}-disabled", State = TagFilterState.Forbidden },
        ];
    }
}