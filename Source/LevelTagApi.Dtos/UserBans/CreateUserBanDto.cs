namespace LevelTagApi.Dtos.UserBans;

public class CreateUserBanDto
{
    public long SteamId { get; set; }

    public string? Reason { get; set; }
}