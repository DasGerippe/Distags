namespace Distags.Contracts;

public class WorkshopLevelDto : LevelDto
{
    public override LevelType Type => LevelType.Workshop;

    public long WorkshopId { get; set; }

    public long CreatorId { get; set; }

    public string? CreatorName { get; set; }

    public string? CreatorAvatarHash { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public List<string> WorkshopTags { get; set; } = [];
}
