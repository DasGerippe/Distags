namespace DataClasses;

public class Contribution
{
    public long Id { get; set; }

    public long LevelId { get; set; }

    public Level Level { get; set; }

    public long TagId { get; set; }

    public Tag Tag { get; set; }

    public long? UserId { get; set; }

    public User? User { get; set; }

    public long? AppId { get; set; }

    public App? App { get; set; }

    public ContributionAction Action { get; set; }

    public DateTime ContributedOn { get; set; }
}