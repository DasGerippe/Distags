using DataClasses;

namespace LevelTagApi.Hubs;

public interface IContributionBroadcaster
{
    Task BroadcastNewContributionAsync(Contribution contribution);

    Task BroadcastNewContributionsAsync(IEnumerable<Contribution> contributions);
}