using DataClasses;
using DataStoring;

namespace LevelTagApi.Management;

public class ContributionManager : IContributionManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IUserManager _UserManager;

    public ContributionManager(LevelTagDbContext dbContext, IUserManager userManager)
    {
        _DbContext = dbContext;
        _UserManager = userManager;
    }

    public async Task CreateAddTagContributionAsync(
        long levelId, long tagId, IEnumerable<long> removedConflictingTagIds)
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        DateTime utcNow = DateTime.UtcNow;

        _DbContext.Add(new Contribution
        {
            LevelId = levelId,
            TagId = tagId,
            User = user,
            Action = ContributionAction.TagAdded,
            ContributedOn = utcNow,
        });

        _DbContext.AddRange(removedConflictingTagIds.Select(removedTagId => new Contribution
        {
            LevelId = levelId,
            TagId = removedTagId,
            User = user,
            Action = ContributionAction.TagRemoved,
            ContributedOn = utcNow,
        }));
    }

    public async Task CreateRemoveTagContributionAsync(long levelId, long tagId)
    {
        User? user = await _UserManager.GetCurrentUserAsync();

        _DbContext.Add(new Contribution
        {
            LevelId = levelId,
            TagId = tagId,
            User = user,
            Action = ContributionAction.TagRemoved,
            ContributedOn = DateTime.UtcNow,
        });
    }
}