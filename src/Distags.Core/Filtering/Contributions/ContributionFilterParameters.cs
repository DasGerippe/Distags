namespace Distags.Core.Filtering.Contributions;

public class ContributionFilterParameters
{
    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public string? LevelSlug { get; set; }

    public long? LevelWorkshopId { get; set; }

    public string? TagName { get; set; }

    public DateTimeOffset? ContributedAfter { get; set; }

    public DateTimeOffset? ContributedBefore { get; set; }

    public bool IgnoreAnonymity { get; set; }
}
