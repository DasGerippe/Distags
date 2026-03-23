using System.Collections.ObjectModel;

namespace LevelTagWebApp.Filters;

public class LevelInfoFilterGroup : ParentFilterGroup
{
    private readonly FilterGroup[] _SubGroups =
    [
        new GameModesFilterGroup(),
        new MiscFilterGroup(),
    ];

    public override string Name => "Level Info";

    public override IReadOnlyList<FilterGroup> SubFilterGroups => new ReadOnlyCollection<FilterGroup>(_SubGroups);
}