namespace LevelTagApi.Dtos.Contributions;

public class LevelDto
{
    public required long Id { get; set; }

    public required string Name { get; set; }

    public required string? CreatorName { get; set; }

    public required string? CreatorAvatarHash { get; set; }

    public required string ImageUrl { get; set; }
}