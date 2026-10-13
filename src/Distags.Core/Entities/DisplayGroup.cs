namespace Distags.Core.Entities;

public class DisplayGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; } = 0;

    public List<Tag> Tags { get; set; } = [];
}
