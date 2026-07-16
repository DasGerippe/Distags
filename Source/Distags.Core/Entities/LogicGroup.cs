namespace Distags.Core.Entities;

public class LogicGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool AllExclusive { get; set; }

    public List<Tag> Tags { get; set; } = [];
}