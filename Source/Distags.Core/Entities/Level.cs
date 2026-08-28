namespace Distags.Core.Entities;

public abstract class Level
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public LevelType Type { get; private set; }

    public List<Tag> Tags { get; } = [];

    public List<LevelTag> LevelTags { get; } = [];

    protected Level(LevelType type)
    {
        Type = type;
    }
}
