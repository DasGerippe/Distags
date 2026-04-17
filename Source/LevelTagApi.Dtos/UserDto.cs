namespace LevelTagApi.Dtos;

public class UserDto
{
    public required long SteamId { get; set; }

    public required string SteamName { get; set; }

    public required string SteamAvatarHash { get; set; }

    public required bool AcceptsGuidelines { get; set; }

    public required bool IsAnonymous { get; set; }

    public required bool IsBanned { get; set; }

    public List<string> Roles { get; set; } = [];
}