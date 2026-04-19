namespace LevelTagApi.Dtos.Tags;

public class TagGroupDto
{
    public required string Name { get; set; }

    public List<TagDto> Tags { get; set; } = [];
}