namespace Distags.Contracts;

public class ContributionUserDto
{
    public required bool IsAnonymous { get; set; }

    public long? SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }

    public bool IsBanned { get; set; }
}
