namespace DataClasses;

public class Creator
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CachedOn { get; set; }

    public List<Level> CreatedLevels { get; set; } = [];
}