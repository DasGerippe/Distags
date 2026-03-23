using System.ComponentModel.DataAnnotations;

namespace SteamDataScraper.Creators;

public class CreatorRefreshSettings
{
    [Range(1, 365)]
    public int RefreshCycleDays { get; set; } = 30;

    [Range(1, 100)]
    public int MinRefreshCount { get; set; } = 10;
}