namespace LevelTagApi.Management;

public interface IContributionManager
{
    Task CreateAddTagContributionAsync(long levelId, long tagId, IEnumerable<long> removedConflictingTagIds);

    Task CreateRemoveTagContributionAsync(long levelId, long tagId);
}