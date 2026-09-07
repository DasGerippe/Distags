namespace Distags.Core.Commands.ImportScrapedLevels;

public class ScrapedLevel
{
    public required long WorkshopId { get; set; }

    public required string Name { get; set; }

    public required string FileName { get; set; }

    public required string FileUrl { get; set; }

    public required string ImageUrl { get; set; }

    public required long CreatorId { get; set; }

    public required DateTimeOffset CreatedOn { get; set; }

    public required DateTimeOffset UpdatedOn { get; set; }

    public required List<string> WorkshopTags { get; set; }
}
