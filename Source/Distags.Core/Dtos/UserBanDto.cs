namespace Distags.Core.Dtos;

public class UserBanDto
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public DateTimeOffset BannedOn { get; set; }

    public UserBanBannerDto Banner { get; set; } = new UserBanBannerDto();

    public string? Reason { get; set; }
}