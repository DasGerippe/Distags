namespace Distags.Core.Entities;

public class UserBan
{
    public long SteamId { get; set; }

    public long? UserId { get; set; }

    public User? User { get; set; }

    public DateTimeOffset BannedOn { get; set; }

    public long BannerId { get; set; }

    public User Banner { get; set; }

    public string? Reason { get; set; }
}
