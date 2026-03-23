namespace LevelTagApi.Dtos;

public class UserBanDto
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public DateTime BannedOn { get; set; }

    public string? Reason { get; set; }
}