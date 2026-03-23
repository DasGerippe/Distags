namespace DataClasses;

public class Tag
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Level> Levels { get; set; } = [];

    public List<LevelTag> LevelTags { get; set; } = [];
}