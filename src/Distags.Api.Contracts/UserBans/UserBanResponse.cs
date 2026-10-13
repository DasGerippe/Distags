namespace Distags.Api.Contracts.UserBans;

public class UserBanResponse
{
    public long SteamId { get; init; }

    public string? SteamName { get; init; }

    public string? SteamAvatarHash { get; init; }

    public DateTimeOffset BannedOn { get; init; }

    public UserBanBannerResponse Banner { get; init; } = new UserBanBannerResponse();

    public string? Reason { get; init; }
}
