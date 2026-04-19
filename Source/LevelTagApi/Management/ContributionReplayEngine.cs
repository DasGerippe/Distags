using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LevelTagApi.Management;

public class ContributionReplayEngine : IContributionReplayEngine
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionChangeCreator _ContributionChangeCreator;

    public ContributionReplayEngine(LevelTagDbContext dbContext, IContributionChangeCreator contributionChangeCreator)
    {
        _DbContext = dbContext;
        _ContributionChangeCreator = contributionChangeCreator;
    }

    public async Task ReplayContributionsAsync(long levelId)
    {
        await using IDbContextTransaction transaction = await _DbContext.Database.BeginTransactionAsync();
        await _DbContext.LockLevelAsync(levelId);

        List<Contribution> contributions = await QueryAllContributionsForLevel(levelId).ToListAsync();
        await ReplayContributionsAsync(levelId, contributions);

        await _DbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private IQueryable<Contribution> QueryAllContributionsForLevel(long levelId)
    {
        return _DbContext.Set<Contribution>()
            .Include(contribution => contribution.Tag)
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.LevelId == levelId)
            .OrderBy(contribution => contribution.ContributedOn);
    }

    private async Task ReplayContributionsAsync(long levelId, List<Contribution> contributions)
    {
        ContributionReplayState replayState = new ContributionReplayState(_ContributionChangeCreator);
        foreach (Contribution contribution in contributions)
        {
            contribution.Changes = replayState.ApplyContribution(contribution);
        }

        List<long> tagIdsForLevel = replayState.GetTagsForLevel()
            .Select(tag => tag.Id)
            .ToList();

        List<long> currentTagIds = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.LevelId == levelId)
            .Select(levelTag => levelTag.TagId)
            .ToListAsync();

        List<LevelTag> removeLevelTags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.LevelId == levelId && !tagIdsForLevel.Contains(levelTag.TagId))
            .ToListAsync();

        List<LevelTag> addLevelTags = tagIdsForLevel.Except(currentTagIds)
            .Select(newTagId => new LevelTag
            {
                LevelId = levelId,
                TagId = newTagId,
            })
            .ToList();

        _DbContext.RemoveRange(removeLevelTags);
        _DbContext.AddRange(addLevelTags);
    }
}