using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters.Abilities;

public class AbilitiesFilterGroup : ParentFilterGroup
{
    private readonly FilterGroup[] _SubGroups =
    [
        new BoostFilterGroup(),
        new JumpFilterGroup(),
        new GripFilterGroup(),
        new WingsFilterGroup(),
    ];

    public override string Name => "Car Abilities";

    public override IReadOnlyList<FilterGroup> SubFilterGroups => new ReadOnlyCollection<FilterGroup>(_SubGroups);
}