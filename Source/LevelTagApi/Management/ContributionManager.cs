using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.EntityFrameworkCore;
using ContributionAction = DataClasses.ContributionAction;

namespace LevelTagApi.Management;

public class ContributionManager : IContributionManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IUserManager _UserManager;
    private readonly IAppManager _AppManager;

    public ContributionManager(LevelTagDbContext dbContext, IUserManager userManager, IAppManager appManager)
    {
        _DbContext = dbContext;
        _UserManager = userManager;
        _AppManager = appManager;
    }

    public async Task CreateContributionsAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags)
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        App? app = await _AppManager.GetCurrentAppAsync();
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

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

    public async Task<RevertContributionsResponse> RevertContributionsAsync(long steamId, DateTimeOffset revertStartDate)
    {
        List<LevelTagContributionGroup> levelTagContributionsGroups =
            await GetContributionsGroupToRevertAsync(steamId, revertStartDate);

        RevertContributionsResponse response = RevertContributions(steamId, levelTagContributionsGroups);

        await _DbContext.SaveChangesAsync();
        return response;
    }

    private async Task<List<LevelTagContributionGroup>> GetContributionsGroupToRevertAsync(long steamId, DateTimeOffset revertStartDate)
    {
        return await _DbContext.Set<Contribution>()
            // Get all Contributions since revertStartDate, grouped by level and tag, where at least one Contribution is from steamId.
            // Those are the groups where Contributions need to be reverted and LevelTags potentially need to be changed.
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.ContributedOn >= revertStartDate)
            .GroupBy(contribution => new { contribution.LevelId, contribution.TagId })
            .Where(group => group.Any(contribution => contribution.User!.SteamId == steamId))
            .Select(group => new { group.Key.LevelId, group.Key.TagId })
            // Left join onto LevelTags so we don't have to fetch them later/separately when we need to add/remove LevelTags.
            .GroupJoin(
                _DbContext.Set<LevelTag>(),
                levelTagKey => new { levelTagKey.LevelId, levelTagKey.TagId },
                levelTag => new { levelTag.LevelId, levelTag.TagId },
                (levelTagKey, levelTags) => new { LevelTagKey = levelTagKey, LevelTags = levelTags })
            .SelectMany(
                keyedLevelTags => keyedLevelTags.LevelTags.DefaultIfEmpty(),
                (keyedLevelTags, levelTag) => new LevelTagContributionGroup
                {
                    LevelId = keyedLevelTags.LevelTagKey.LevelId,
                    TagId = keyedLevelTags.LevelTagKey.TagId,
                    LevelTag = levelTag,
                    Contributions = _DbContext.Set<Contribution>()
                        .Where(contribution =>
                            !contribution.IsReverted &&
                            contribution.ContributedOn >= revertStartDate &&
                            contribution.LevelId == keyedLevelTags.LevelTagKey.LevelId &&
                            contribution.TagId == keyedLevelTags.LevelTagKey.TagId)
                        .OrderBy(contribution => contribution.Id)
                        .ToList(),
                })
            .ToListAsync();
    }

    private RevertContributionsResponse RevertContributions(long steamId, List<LevelTagContributionGroup> contributionGroups)
    {
        RevertContributionsResponse response = new RevertContributionsResponse();

        foreach (LevelTagContributionGroup contributionGroup in contributionGroups)
        {
            long levelId = contributionGroup.LevelId;
            long tagId = contributionGroup.TagId;
            LevelTag? levelTag = contributionGroup.LevelTag;

            bool revertNextContribution = false;
            bool levelTagShouldExist = false;
            foreach (Contribution contribution in contributionGroup.Contributions)
            {
                if (revertNextContribution)
                {
                    // Previous contribution was reverted. Revert the current contribution as well, regardless of contributor,
                    // because it can't exist without the previous reverted contribution.
                    contribution.IsReverted = true;
                    revertNextContribution = false;
                    if (contribution.User?.SteamId == steamId)
                        response.ContributionsReverted++;
                    else
                        response.CollateralContributionsReverted++;
                }
                else if (contribution.User?.SteamId == steamId)
                {
                    contribution.IsReverted = true;
                    revertNextContribution = true;
                    response.ContributionsReverted++;

                    levelTagShouldExist = contribution.Action == ContributionAction.TagRemoved;
                }
                else
                {
                    levelTagShouldExist = contribution.Action == ContributionAction.TagAdded;
                }
            }

            if (levelTagShouldExist && levelTag == null)
            {
                _DbContext.Add(new LevelTag { LevelId = levelId, TagId = tagId });
                response.LevelTagsAdded++;
            }
            else if (!levelTagShouldExist && levelTag != null)
            {
                _DbContext.Remove(levelTag);
                response.LevelTagsRemoved++;
            }
        }

        return response;
    }

    private class LevelTagContributionGroup
    {
        public long LevelId { get; init; }

        public long TagId { get; init; }

        public List<Contribution> Contributions { get; init; } = [];

        public LevelTag? LevelTag { get; init; }
    }
}