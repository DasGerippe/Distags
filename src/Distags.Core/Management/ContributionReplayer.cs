using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Interfaces;
using Distags.Core.Logic;
using Distags.Core.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Management;

public class ContributionReplayer
{
    private readonly DistagsDbContext _DbContext;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly FallbackTagProvider _FallbackTagProvider;
    private readonly LevelTagAdder _LevelTagAdder;
    private readonly LevelTagRemover _LevelTagRemover;
    private readonly AppEventQueue _EventQueue;

    public ContributionReplayer(
        DistagsDbContext dbContext,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        FallbackTagProvider fallbackTagProvider,
        LevelTagAdder levelTagAdder,
        LevelTagRemover levelTagRemover,
        AppEventQueue eventQueue)
    {
        _DbContext = dbContext;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _FallbackTagProvider = fallbackTagProvider;
        _LevelTagAdder = levelTagAdder;
        _LevelTagRemover = levelTagRemover;
        _EventQueue = eventQueue;
    }

    public async Task ReplayContributionsAsync(long levelId)
    {
        await _SequentialLevelActionExecutor.ExecuteSequentialForLevelAsync(levelId, async () =>
        {
            Level level = await _DbContext.Set<Level>()
                .Include(level => level.Tags)
                .FirstAsync(level => level.Id == levelId)
                .ConfigureAwait(false);

            List<Contribution> contributions = await GetAllContributionsForLevelAsync(levelId).ConfigureAwait(false);
            await ReplayContributionsAsync(level, contributions).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private Task<List<Contribution>> GetAllContributionsForLevelAsync(long levelId)
    {
        return _DbContext.Set<Contribution>()
            .Include(contribution => contribution.Tag.LogicGroup!.Tags)
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.LevelId == levelId)
            .OrderBy(contribution => contribution.ContributedOn)
            .ToListAsync();
    }

    private async Task ReplayContributionsAsync(Level level, List<Contribution> contributions)
    {
        List<Tag> originalTags = level.Tags.ToList();
        level.Tags.Clear();

        IReadOnlyCollection<Tag> fallbackTags = await _FallbackTagProvider.GetFallbackTagsAsync().ConfigureAwait(false);
        level.Tags.AddRange(fallbackTags);

        foreach (Contribution contribution in contributions)
        {
            DomainResult<List<TagChange>> result = contribution.Action == TagChangeAction.Add
                ? _LevelTagAdder.AddTagToLevel(level, contribution.Tag)
                : _LevelTagRemover.RemoveTagFromLevel(level, contribution.Tag);

            contribution.Changes = result.IsSuccess ? result.Value : [];
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);

        EnqueueLevelTagsChangedEvent(level, originalTags);
    }

    private void EnqueueLevelTagsChangedEvent(Level level, List<Tag> originalTags)
    {
        IEnumerable<TagChangeDto> addChanges = level.Tags
            .Except(originalTags)
            .Select(tag => new TagChangeDto
            {
                Action = TagChangeAction.Add,
                TagName = tag.Name,
            });

        IEnumerable<TagChangeDto> removeChanges = originalTags
            .Except(level.Tags)
            .Select(tag => new TagChangeDto
            {
                Action = TagChangeAction.Remove,
                TagName = tag.Name,
            });

        List<TagChangeDto> allChanges = addChanges.Concat(removeChanges).ToList();

        _EventQueue.Enqueue(new LevelTagsChangedEvent(LevelMapper.Map(level), allChanges));
    }
}
