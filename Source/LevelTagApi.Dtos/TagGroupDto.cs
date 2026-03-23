namespace LevelTagApi.Dtos;

public class TagGroupDto
{
    public required string Name { get; set; }

    public List<TagDto> Tags { get; set; } = [];
}