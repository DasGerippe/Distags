using DataClasses;

namespace LevelTagApi.Management;

public interface IContributionManager
{
    Task<Contribution> CreateContributionAsync(Level level, Tag tag, ChangeAction action, List<ContributionChange> changes);
}