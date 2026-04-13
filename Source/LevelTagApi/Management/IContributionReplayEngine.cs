namespace LevelTagApi.Management;

public interface IContributionReplayEngine
{
    Task ReplayContributionsAsync(long levelId);
}