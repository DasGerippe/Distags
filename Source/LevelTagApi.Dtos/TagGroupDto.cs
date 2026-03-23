namespace LevelTagApi.Dtos;

public class TagGroupDto
{
    public required string Name { get; set; }

    public required TagCompatibility TagCompatibility { get; set; }

    public List<TagDto> Tags { get; set; } = [];

    public List<TagGroupDto> SubGroups { get; set; } = [];
}