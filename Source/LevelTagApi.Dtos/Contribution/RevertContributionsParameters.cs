namespace LevelTagApi.Dtos.Contribution;

public class RevertContributionsParameters
{
    public long SteamId { get; set; }

    public DateTimeOffset? RevertStartDate { get; set; }
}