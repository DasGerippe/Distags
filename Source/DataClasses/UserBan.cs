namespace DataClasses;

public class UserBan
{
    public long SteamId { get; set; }

    public DateTime BannedOn { get; set; }

    public long BannerId { get; set; }

    public User Banner { get; set; }

    public string? Reason { get; set; }
}