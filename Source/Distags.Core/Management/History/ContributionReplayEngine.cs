using DataClasses;
using Distags.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Management.History;

public class ContributionReplayEngine
{
    private readonly DbContext _DbContext;
    private readonly ContributionChangeManager _ContributionChangeManager;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;

    public ContributionReplayEngine(
        DbContext dbContext,
        ContributionChangeManager contributionChangeManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor)
    {
        _DbContext = dbContext;
        _ContributionChangeManager = contributionChangeManager;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
    }

    public async Task ReplayContributionsAsync(long levelId)
    {
        await _SequentialLevelActionExecutor.ExecuteSequentialForLevelAsync(levelId, async () =>
        {
            List<Contribution> contributions = await GetAllContributionsForLevel(levelId).ConfigureAwait(false);
            await ReplayContributionsAsync(levelId, contributions).ConfigureAwait(false);

            await _DbContext.SaveChangesAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private Task<List<Contribution>> GetAllContributionsForLevel(long levelId)
    {
        return _DbContext.Set<Contribution>()
            .Include(contribution => contribution.Tag)
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.LevelId == levelId)
            .OrderBy(contribution => contribution.ContributedOn)
            .ToListAsync();
    }

    private async Task ReplayContributionsAsync(long levelId, List<Contribution> contributions)
    {
        ContributionReplayState replayState = new ContributionReplayState(_ContributionChangeManager);
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
            .ToListAsync().ConfigureAwait(false);

        List<LevelTag> removeLevelTags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.LevelId == levelId && !tagIdsForLevel.Contains(levelTag.TagId))
            .ToListAsync().ConfigureAwait(false);

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