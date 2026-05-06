using DataClasses;
using Distags.Contracts;
using Distags.Core.Events;
using Distags.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Management.History;

public class ContributionReplayEngine
{
    private readonly DistagsDbContext _DbContext;
    private readonly ContributionChangeManager _ContributionChangeManager;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly AppEventQueue _EventQueue;

    public ContributionReplayEngine(
        DistagsDbContext dbContext,
        ContributionChangeManager contributionChangeManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        AppEventQueue eventQueue)
    {
        _DbContext = dbContext;
        _ContributionChangeManager = contributionChangeManager;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _EventQueue = eventQueue;
    }

    public async Task ReplayContributionsAsync(long levelId)
    {
        await _SequentialLevelActionExecutor.ExecuteSequentialForLevelAsync(levelId, async () =>
        {
            List<Contribution> contributions = await GetAllContributionsForLevel(levelId).ConfigureAwait(false);
            await ReplayContributionsAsync(levelId, contributions).ConfigureAwait(false);
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

        List<Tag> tagsForLevel = replayState.GetTagsForLevel().ToList();
        List<Tag> currentTags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.LevelId == levelId)
            .Select(levelTag => levelTag.Tag)
            .ToListAsync().ConfigureAwait(false);

        List<LevelTag> removeLevelTags = await _DbContext.Set<LevelTag>()
            .Include(levelTag => levelTag.Tag)
            .Where(levelTag => levelTag.LevelId == levelId && !tagsForLevel.Contains(levelTag.Tag))
            .ToListAsync().ConfigureAwait(false);

        List<LevelTag> addLevelTags = tagsForLevel.Except(currentTags)
            .Select(newTag => new LevelTag
            {
                LevelId = levelId,
                Tag = newTag,
            })
            .ToList();

        _DbContext.RemoveRange(removeLevelTags);
        _DbContext.AddRange(addLevelTags);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);

        IEnumerable<TagChangeDto> addChanges = addLevelTags
            .Select(levelTag => new TagChangeDto()
            {
                ChangeType = TagChangeType.Add,
                TagName = levelTag.Tag.Name,
            });
        IEnumerable<TagChangeDto> removeChanges = removeLevelTags
            .Select(levelTag => new TagChangeDto
            {
                ChangeType = TagChangeType.Remove,
                TagName = levelTag.Tag.Name,
            });
        _EventQueue.Enqueue(new LevelTagsChangedEvent(levelId, addChanges.Concat(removeChanges).ToList()));
    }
}