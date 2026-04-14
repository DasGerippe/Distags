namespace LevelTagApi.Filtering;

public class ContributionFilterParameters
{
    public long? UserSteamId { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    public bool IgnoreAnonymity { get; set; }
}