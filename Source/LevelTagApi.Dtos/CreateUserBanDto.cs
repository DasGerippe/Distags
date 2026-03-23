namespace LevelTagApi.Dtos;

public class CreateUserBanDto
{
    public long SteamId { get; set; }

    public string? Reason { get; set; }
}