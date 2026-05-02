namespace Distags.Core.Filtering.Contributions;

public class ContributionFilterParameters
{
    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    public bool IgnoreAnonymity { get; set; }
}