namespace DataClasses;

public class User
{
    public long Id { get; set; }

    public long? SteamId { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<UserSession> Sessions { get; set; } = [];
}