namespace Distags.Api.Contracts.Users;

public class UserResponse
{
    public long SteamId { get; init; }

    public string? SteamName { get; init; }

    public string? SteamAvatarHash { get; init; }

    public bool AcceptsGuidelines { get; init; }

    public bool IsAnonymous { get; init; }

    public bool IsBanned { get; init; }

    public List<string> Roles { get; init; } = [];
}
