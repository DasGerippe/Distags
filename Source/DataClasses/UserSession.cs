namespace DataClasses;

public class UserSession
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public User User { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresOn { get; set; }
}