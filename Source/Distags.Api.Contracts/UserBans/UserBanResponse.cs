namespace Distags.Api.Contracts.UserBans;

public class UserBanResponse
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public DateTimeOffset BannedOn { get; set; }

    public UserBanBannerResponse Banner { get; set; } = new UserBanBannerResponse();

    public string? Reason { get; set; }
}
