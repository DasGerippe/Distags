namespace LevelTagApi.Dtos;

public class UserDto
{
    public long SteamId { get; set; }

    public List<string> Roles { get; set; } = [];
}