namespace LevelTagApi.Dtos;

public class RevertContributionsParameters
{
    public long SteamId { get; set; }

    public DateTime? RevertStartDate { get; set; }
}