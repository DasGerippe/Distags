namespace LevelTagWebApp.Filters;

public class TagFilter
{
    public required string Name { get; set; }

    public required string Tag { get; set; }

    public TagFilterState State { get; set; } = TagFilterState.Neutral;
}