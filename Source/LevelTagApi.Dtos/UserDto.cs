namespace LevelTagApi.Dtos;

public class UserDto
{
    public required long SteamId { get; set; }

    public required string SteamName { get; set; }

    public required string SteamAvatarHash { get; set; }

    public List<string> Roles { get; set; } = [];
}