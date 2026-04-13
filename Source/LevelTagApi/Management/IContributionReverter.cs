namespace LevelTagApi.Management;

public interface IContributionReverter
{
    Task RevertContributionsAsync(long steamId, DateTimeOffset revertStartDate);
}