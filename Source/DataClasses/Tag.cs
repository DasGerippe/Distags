namespace DataClasses;

public class Tag
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; } = 0;

    public long? GroupId { get; set; }

    public TagGroup? Group { get; set; }

    public List<Level> Levels { get; set; } = [];

    public List<LevelTag> LevelTags { get; set; } = [];
}