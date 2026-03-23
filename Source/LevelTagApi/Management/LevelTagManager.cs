using DataClasses;
using DataStoring;
using LevelTagApi.Hubs;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionManager _ContributionManager;
    private readonly ILevelHubServer _LevelHubServer;

    public LevelTagManager(
        LevelTagDbContext dbContext,
        IContributionManager contributionManager,
        ILevelHubServer levelHubServer)
    {
        _DbContext = dbContext;
        _ContributionManager = contributionManager;
        _LevelHubServer = levelHubServer;
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
            await _LevelHubServer.SendTagRemovedMessageAsync(level.Id, removedTag.Name);
        }
        await _LevelHubServer.SendTagAddedMessageAsync(level.Id, tag.Name);
    }

    public async Task RemoveTagFromLevelAsync(LevelTag levelTag)
    {
        await _ContributionManager.CreateRemoveTagContributionAsync(levelTag.LevelId, levelTag.TagId);

        _DbContext.Remove(levelTag);
        await _DbContext.SaveChangesAsync();

        // Make sure it's safe to access levelTag.Tag.Name
        await _DbContext.Entry(levelTag).Reference(levelTag => levelTag.Tag).LoadAsync();
        await _LevelHubServer.SendTagRemovedMessageAsync(levelTag.LevelId, levelTag.Tag.Name);
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