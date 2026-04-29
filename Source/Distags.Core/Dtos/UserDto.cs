namespace Distags.Core.Dtos;

public class UserDto
{
    public long SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public bool AcceptsGuidelines { get; set; }

    public bool IsAnonymous { get; set; }

    public bool IsBanned { get; set; }

    public List<string> Roles { get; set; } = [];
}