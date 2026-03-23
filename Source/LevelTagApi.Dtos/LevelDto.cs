namespace LevelTagApi.Dtos;

public class LevelDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public long CreatorId { get; set; }

    public string? CreatorName { get; set; }

    public string? CreatorAvatarHash { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public List<string> Tags { get; set; } = [];
}