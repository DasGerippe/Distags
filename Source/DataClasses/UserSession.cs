namespace DataClasses;

public class UserSession
{
    public string Token { get; set; } = string.Empty;

    public long UserId { get; set; }

    public User User { get; set; }

    public DateTimeOffset LastSeenOn { get; set; }
}