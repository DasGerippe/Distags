namespace Distags.Core.Entities;

public class User
{
    public long Id { get; set; }

    public long? SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public bool IsBanned { get; set; }

    public bool AcceptsGuidelines { get; set; }

    public bool IsAnonymous { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<UserSession> Sessions { get; set; } = [];
}