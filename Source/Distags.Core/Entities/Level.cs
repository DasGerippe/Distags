namespace Distags.Core.Entities;

public class Level
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public long CreatorId { get; set; }

    public Creator? Creator { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }

    public List<string> WorkshopTags { get; set; } = [];

    public List<Tag> Tags { get; } = [];

    public List<LevelTag> LevelTags { get; } = [];
}