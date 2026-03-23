namespace LevelTagApi.Dtos;

public class TagDto
{
    public required string Name { get; set; }

    public required string DisplayName { get; set; }

    public int? Exclusivity { get; set; }

    public bool IsAutoTag { get; set; }
}