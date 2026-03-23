namespace LevelTagApi.Dtos;

public class UserDto
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public List<string> Roles { get; set; } = [];
}