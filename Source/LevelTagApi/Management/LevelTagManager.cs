using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;

    public LevelTagManager(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task AddTagToLevelAsync(Level level, Tag tag)
    {
        await RemoveConflictingTagsAsync(level, tag);

        _DbContext.Add(new LevelTag
        {
            Level = level,
            Tag = tag,
        });
        await _DbContext.SaveChangesAsync();
    }

    public async Task RemoveTagFromLevelAsync(LevelTag levelTag)
    {
        _DbContext.Remove(levelTag);
        await _DbContext.SaveChangesAsync();
    }

    private async Task RemoveConflictingTagsAsync(Level level, Tag tag)
    {
        if (tag.Exclusivity == null)
            return;

        List<LevelTag> conflictingLevelTags = await _DbContext.Set<LevelTag>()
            .Where(levelTag => levelTag.Level == level && levelTag.Tag.Exclusivity == tag.Exclusivity)
            .ToListAsync();

        _DbContext.RemoveRange(conflictingLevelTags);
    }
}