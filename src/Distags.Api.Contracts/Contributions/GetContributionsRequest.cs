using System.ComponentModel.DataAnnotations;

namespace Distags.Api.Contracts.Contributions;

public class GetContributionsRequest
{
    public long? UserSteamId { get; init; }

    public string? AppName { get; init; }

    public string? LevelSlug { get; init; }

    public long? LevelWorkshopId { get; init; }

    public string? TagName { get; init; }

    public DateTimeOffset? ContributedAfter { get; init; }

    public DateTimeOffset? ContributedBefore { get; init; }

    public bool DescendingOrder { get; init; } = true;

    [Range(1, 100, ErrorMessage = $"{nameof(Limit)} must be between 1 and 100.")]
    public int Limit { get; init; } = 100;
}
