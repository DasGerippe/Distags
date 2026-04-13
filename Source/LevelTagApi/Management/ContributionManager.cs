using DataClasses;

namespace LevelTagApi.Management;

public class ContributionManager : IContributionManager
{
    private readonly IUserManager _UserManager;
    private readonly IAppManager _AppManager;

    public ContributionManager(
        IUserManager userManager,
        IAppManager appManager)
    {
        _UserManager = userManager;
        _AppManager = appManager;
    }

    public async Task<Contribution> CreateContributionAsync(Level level, Tag tag, ChangeAction action, List<ContributionChange> changes)
    {
        return new Contribution
        {
            Level = level,
            Tag = tag,
            Action = action,
            User = await _UserManager.GetCurrentUserAsync(),
            App = await _AppManager.GetCurrentAppAsync(),
            ContributedOn = DateTimeOffset.UtcNow,
            Changes = changes,
        };
    }
}