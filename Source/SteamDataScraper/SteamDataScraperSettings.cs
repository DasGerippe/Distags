using System.ComponentModel.DataAnnotations;

namespace SteamDataScraper;

public class SteamDataScraperSettings
{
    [Range(1, 365)]
    public int CreatorRefreshCycleDays { get; set; } = 30;

    [Range(1, 100)]
    public int MinCreatorRefreshCount { get; set; } = 10;
}