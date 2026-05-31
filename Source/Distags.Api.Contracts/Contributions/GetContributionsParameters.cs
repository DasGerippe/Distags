using System.ComponentModel.DataAnnotations;

namespace Distags.Api.Contracts.Contributions;

public class GetContributionsParameters
{
    public long? UserSteamId { get; set; }

    public string? AppName { get; set; }

    public long? LevelId { get; set; }

    public string? TagName { get; set; }

    public DateTimeOffset? ContributedAfter { get; set; }

    public DateTimeOffset? ContributedBefore { get; set; }

    public bool DescendingOrder { get; set; } = true;

    [Range(1, 100, ErrorMessage = $"{nameof(Limit)} must be between 1 and 100.")]
    public int Limit { get; set; } = 100;
}