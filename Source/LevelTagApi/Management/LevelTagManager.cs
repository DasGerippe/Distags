using DataClasses;
using DataStoring;
using LevelTagApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LevelTagApi.Management;

public class LevelTagManager : ILevelTagManager
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionManager _ContributionManager;
    private readonly IContributionChangeCreator _ContributionChangeCreator;

    public LevelTagManager(
        LevelTagDbContext dbContext,
        IContributionManager contributionManager,
        IContributionChangeCreator contributionChangeCreator)
    {
        _DbContext = dbContext;
        _ContributionManager = contributionManager;
        _ContributionChangeCreator = contributionChangeCreator;
    }

    public async Task AddTagToLevelAsync(long levelId, string tagName)
    {
        Level level = await GetLevelByIdAsync(levelId);
        Tag tag = await GetTagByNameAsync(tagName);

        await using IDbContextTransaction transaction = await _DbContext.Database.BeginTransactionAsync();
        await _DbContext.LockLevelAsync(levelId);
        
        LevelTag? levelTag = await _DbContext.FindAsync<LevelTag>(levelId, tag.Id);
        if (levelTag != null)
            throw new LevelTagExistsException(levelId, tagName);

        List<LevelTag> conflictingLevelTags = await GetConflictingLevelTagsAsync(level, tag);
        LevelTag addedLevelTag = new LevelTag
        {
            Level = level,
            Tag = tag,
        };

        List<ContributionChange> changes = _ContributionChangeCreator.CreateChanges(
            tag, conflictingLevelTags.Select(removedLevelTag => removedLevelTag.Tag));
        Contribution contribution = await _ContributionManager.CreateContributionAsync(level, tag, ChangeAction.AddTag, changes);

        _DbContext.RemoveRange(conflictingLevelTags);
        _DbContext.Add(addedLevelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task RemoveTagFromLevelAsync(long levelId, string tagName)
    {
        Level level = await GetLevelByIdAsync(levelId);
        Tag tag = await GetTagByNameAsync(tagName);

        await using IDbContextTransaction transaction = await _DbContext.Database.BeginTransactionAsync();
        await _DbContext.LockLevelAsync(levelId);

        LevelTag? levelTag = await _DbContext.FindAsync<LevelTag>(levelId, tag.Id);
        if (levelTag == null)
            throw new LevelTagNotFoundException(levelId, tagName);

        ContributionChange change = _ContributionChangeCreator.CreateChange(ChangeAction.RemoveTag, tag);
        Contribution contribution = await _ContributionManager.CreateContributionAsync(level, tag, ChangeAction.RemoveTag, [change]);

        _DbContext.Remove(levelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task<Level> GetLevelByIdAsync(long levelId)
    {
        Level? level = await _DbContext.FindAsync<Level>(levelId);
        return level ?? throw new UnknownLevelException(levelId);
    }

    private async Task<Tag> GetTagByNameAsync(string tagName)
    {
        Tag? tag = await _DbContext.Set<Tag>().FirstOrDefaultAsync(tag => tag.Name == tagName);
        return tag ?? throw new UnknownTagException(tagName);
    }

    private async Task<List<LevelTag>> GetConflictingLevelTagsAsync(Level level, Tag tag)
    {
        if (tag.Exclusivity == null)
            return [];

        List<LevelTag> conflictingLevelTags = await _DbContext.Set<LevelTag>()
            .Include(levelTag => levelTag.Tag)
            .Where(levelTag =>
                levelTag.Level == level &&
                levelTag.Tag.GroupId == tag.GroupId &&
                levelTag.Tag.Exclusivity == tag.Exclusivity)
            .ToListAsync();

        return conflictingLevelTags;
    }
}