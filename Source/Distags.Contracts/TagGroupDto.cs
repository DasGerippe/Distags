namespace Distags.Contracts;

public class TagGroupDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }

    public List<TagDto> Tags { get; set; } = [];
}