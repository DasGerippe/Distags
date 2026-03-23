namespace LevelTagApi.Dtos;

public class UserDto
{
    public required long SteamId { get; set; }

    public required string SteamName { get; set; }

    public required string SteamAvatarHash { get; set; }

    public required bool AcceptedContributionGuidelines { get; set; }

    public required bool IsAnonymous { get; set; }

    public List<string> Roles { get; set; } = [];
}