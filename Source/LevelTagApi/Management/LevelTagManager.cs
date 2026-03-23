using DataClasses;
using DataStoring;
using LevelTagApi.Hubs;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionManager _ContributionManager;
    private readonly ILevelTagBroadcaster _LevelTagBroadcaster;

    public LevelTagManager(
        LevelTagDbContext dbContext,
        IContributionManager contributionManager,
        ILevelTagBroadcaster levelTagBroadcaster)
    {
        _DbContext = dbContext;
        _ContributionManager = contributionManager;
        _LevelTagBroadcaster = levelTagBroadcaster;
    }

    public async Task AddTagToLevelAsync(Level level, Tag tag)
    {
        List<Tag> removedTags = await RemoveConflictingTagsAsync(level, tag);

        IEnumerable<long> removedConflictingTagIds = removedTags.Select(removedTag => removedTag.Id);
        await _ContributionManager.CreateAddTagContributionAsync(level.Id, tag.Id, removedConflictingTagIds);

        _DbContext.Add(new LevelTag
        {
            Level = level,
            Tag = tag,
        });
        await _DbContext.SaveChangesAsync();

        foreach (Tag removedTag in removedTags)
        {
            await _LevelTagBroadcaster.BroadcastLevelTagRemovedAsync(level.Id, removedTag.Name);
        }
        await _LevelTagBroadcaster.BroadcastLevelTagAddedAsync(level.Id, tag.Name);
    }

    public async Task RemoveTagFromLevelAsync(LevelTag levelTag)
    {
        await _ContributionManager.CreateRemoveTagContributionAsync(levelTag.LevelId, levelTag.TagId);

        _DbContext.Remove(levelTag);
        await _DbContext.SaveChangesAsync();

        // Make sure it's safe to access levelTag.Tag.Name
        await _DbContext.Entry(levelTag).Reference(levelTag => levelTag.Tag).LoadAsync();
        await _LevelTagBroadcaster.BroadcastLevelTagRemovedAsync(levelTag.LevelId, levelTag.Tag.Name);
    }

    private async Task<List<Tag>> RemoveConflictingTagsAsync(Level level, Tag tag)
    {
        if (tag.Exclusivity == null)
            return [];

        List<LevelTag> conflictingLevelTags = await _DbContext.Set<LevelTag>()
            .Include(levelTag => levelTag.Tag)
            .Where(levelTag => levelTag.Level == level && levelTag.Tag.Exclusivity == tag.Exclusivity)
            .ToListAsync();

        foreach (LevelTag conflictingLevelTag in conflictingLevelTags)
        {
            _DbContext.Remove(conflictingLevelTag);
        }

        return conflictingLevelTags.Select(levelTag => levelTag.Tag).ToList();
    }
}