namespace Distags.Contracts;

public abstract class LevelDto
{
    public abstract LevelType Type { get; }

    public string Name { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];
}
