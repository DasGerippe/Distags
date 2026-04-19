namespace LevelTagApi.Dtos.UserBans;

public class UserBanDto
{
    public required long SteamId { get; set; }

    public required string? SteamName { get; set; }

    public required string? SteamAvatarHash { get; set; }

    public required DateTimeOffset BannedOn { get; set; }

    public required UserDisplayDto Banner { get; set; }

    public required string? Reason { get; set; }
}