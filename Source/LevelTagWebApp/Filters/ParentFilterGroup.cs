namespace LevelTagWebApp.Filters;

public abstract class ParentFilterGroup : FilterGroupBase
{
    public abstract IReadOnlyList<FilterGroup> SubFilterGroups { get; }
}