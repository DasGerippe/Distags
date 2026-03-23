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
    public async Task CreateContributionsAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags)
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        App? app = null;
        DateTime utcNow = DateTime.UtcNow;

        _DbContext.AddRange(removedLevelTags.Select(levelTag => new Contribution
        {
            LevelId = levelTag.LevelId,
            TagId = levelTag.TagId,
            User = user,
            App = app,
            Action = ContributionAction.TagRemoved,
            ContributedOn = utcNow,
        }));

        _DbContext.AddRange(addedLevelTags.Select(levelTag => new Contribution
        {
            LevelId = levelTag.LevelId,
            TagId = levelTag.TagId,
            User = user,
            App = app,
            Action = ContributionAction.TagAdded,
            ContributedOn = utcNow,
        }));
    }
}