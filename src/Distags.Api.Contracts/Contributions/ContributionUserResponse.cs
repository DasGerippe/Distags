namespace Distags.Api.Contracts.Contributions;

public class ContributionUserResponse
{
    public required bool IsAnonymous { get; init; }

    public long? SteamId { get; init; }

    public string? SteamName { get; init; }

    public string? SteamAvatarHash { get; init; }

    public bool IsBanned { get; init; }
}
