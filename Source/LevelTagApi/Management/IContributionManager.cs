using DataClasses;
using LevelTagApi.Dtos;

namespace LevelTagApi.Management;

public interface IContributionManager
{
    Task CreateContributionsAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags);

    Task<RevertContributionsResponse> RevertContributionsAsync(long steamId, DateTimeOffset revertStartDate);
}