namespace LevelTagApi.Dtos;

public class ContributorUserDto : ContributorDto
{
    public bool IsAnonymous { get; set; }

    public long? SteamId { get; set; }

    public string? SteamName { get; set; }

    public string? SteamAvatarHash { get; set; }
}