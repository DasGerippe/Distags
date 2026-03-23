namespace DataClasses;

public class UserBan
{
    public long SteamId { get; set; }

    public DateTime BannedOn { get; set; }

    public string? Reason { get; set; }
}