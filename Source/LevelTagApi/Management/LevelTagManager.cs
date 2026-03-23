using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionManager _ContributionManager;

    public LevelTagManager(LevelTagDbContext dbContext, IContributionManager contributionManager)
    {
        _DbContext = dbContext;
        _ContributionManager = contributionManager;
    }

    public async Task AddTagToLevelAsync(Level level, Tag tag)
    {
        List<LevelTag> removedLevelTags = await RemoveConflictingTagsAsync(level, tag);
        
        LevelTag addedLevelTag = new LevelTag
        {
            Level = level,
            Tag = tag,
        };
        _DbContext.Add(addedLevelTag);

        await _ContributionManager.CreateContributionsAsync([addedLevelTag], removedLevelTags);
        await _DbContext.SaveChangesAsync();
    }

    public async Task RemoveTagFromLevelAsync(LevelTag levelTag)
    {
        _DbContext.Remove(levelTag);
        await _DbContext.SaveChangesAsync();
    }

    private async Task<List<LevelTag>> RemoveConflictingTagsAsync(Level level, Tag tag)
    {
        if (tag.Exclusivity == null)
            return [];

        List<LevelTag> conflictingLevelTags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.Level == level && levelTag.Tag.Exclusivity == tag.Exclusivity)
            .ToListAsync();

        _DbContext.RemoveRange(conflictingLevelTags);
        return conflictingLevelTags;
    }
}