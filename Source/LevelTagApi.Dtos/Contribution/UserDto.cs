namespace LevelTagApi.Dtos.Contribution;

public class UserDto
{
    public required bool IsAnonymous { get; set; }

    public long? SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }
}