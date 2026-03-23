using DataClasses;
using DataStoring;
using LevelTagApi.Hubs;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly ILevelHubServer _LevelHubServer;

    public LevelTagManager(LevelTagDbContext dbContext, ILevelHubServer levelHubServer)
    {
        _DbContext = dbContext;
        _LevelHubServer = levelHubServer;
    }

    public async Task AddTagToLevelAsync(Level level, Tag tag)
    {
        List<string> removedTags = await RemoveConflictingTagsAsync(level, tag);

        _DbContext.Add(new LevelTag
        {
            Level = level,
            Tag = tag,
        });
        await _DbContext.SaveChangesAsync();

        foreach (string removedTag in removedTags)
        {
            await _LevelHubServer.SendTagRemovedMessageAsync(level.Id, removedTag);
        }
        await _LevelHubServer.SendTagAddedMessageAsync(level.Id, tag.Name);
    }

    public async Task RemoveTagFromLevelAsync(LevelTag levelTag)
    {
        _DbContext.Remove(levelTag);
        await _DbContext.SaveChangesAsync();

        // Make sure it's safe to access levelTag.Tag.Name
        await _DbContext.Entry(levelTag).Reference(levelTag => levelTag.Tag).LoadAsync();
        await _LevelHubServer.SendTagRemovedMessageAsync(levelTag.LevelId, levelTag.Tag.Name);
    }

    private async Task<List<string>> RemoveConflictingTagsAsync(Level level, Tag tag)
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

        return conflictingLevelTags.Select(levelTag => levelTag.Tag.Name).ToList();
    }
}