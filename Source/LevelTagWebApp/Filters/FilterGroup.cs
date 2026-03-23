namespace LevelTagWebApp.Filters;

public abstract class FilterGroup : FilterGroupBase
{
    public abstract FilterGroupType FilterGroupType { get; }

    public abstract IReadOnlyList<TagFilter> TagFilters { get; }
}