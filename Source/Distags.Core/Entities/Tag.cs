namespace Distags.Core.Entities;

public class Tag
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; } = 0;

    public bool IsExclusive { get; set; }

    public bool IsFallback { get; set; }

    public bool IsAutoTag { get; set; }

    public long DisplayGroupId { get; set; }

    public DisplayGroup DisplayGroup { get; set; }

    public long? LogicGroupId { get; set; }

    public LogicGroup? LogicGroup { get; set; }

    public List<Level> Levels { get; set; } = [];

    public List<LevelTag> LevelTags { get; set; } = [];
}
