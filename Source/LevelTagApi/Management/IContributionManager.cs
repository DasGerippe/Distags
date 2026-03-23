using DataClasses;

namespace LevelTagApi.Management;

public interface IContributionManager
{
    Task CreateContributionsAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags);
}