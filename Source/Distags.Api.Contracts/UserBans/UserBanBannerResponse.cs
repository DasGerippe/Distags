namespace Distags.Api.Contracts.UserBans;

public class UserBanBannerResponse
{
    public long? SteamId { get; init; }

    public string? SteamName { get; init; }

    public string? SteamAvatarHash { get; init; }
}
