namespace Distags.Core.Entities;

public class WorkshopLevel : Level
{
    public long WorkshopId { get; set; }

    public long CreatorId { get; set; }

    public Creator? Creator { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public List<string> WorkshopTags { get; set; } = [];

    public WorkshopLevel() : base(LevelType.Workshop)
    {
    }
}
