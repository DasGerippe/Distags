namespace LevelTagApi.Dtos.Contributions;

public class RevertContributionsParameters
{
    public long SteamId { get; set; }

    public DateTimeOffset? RevertStartDate { get; set; }
}