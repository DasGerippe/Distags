using DataClasses;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.LevelAddTag;

public class LevelAddTagCommandHandler : ICommandHandler<LevelAddTagCommand>
{
    private readonly DbContext _DbContext;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly ContributionManager _ContributionManager;
    private readonly ContributionChangeManager _ContributionChangeManager;

    public LevelAddTagCommandHandler(
        DbContext dbContext,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        ContributionManager contributionManager,
        ContributionChangeManager contributionChangeManager)
    {
        _DbContext = dbContext;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _ContributionManager = contributionManager;
        _ContributionChangeManager = contributionChangeManager;
    }

    public async Task HandleAsync(LevelAddTagCommand command)
    {
        Level level = await _DbContext.FindAsync<Level>(command.LevelId).ConfigureAwait(false)
            ?? throw new UnknownLevelException(command.LevelId);
        Tag tag = await _DbContext.Set<Tag>().FirstOrDefaultAsync(tag => tag.Name == command.TagName).ConfigureAwait(false)
            ?? throw new UnknownTagException(command.TagName);

        await _SequentialLevelActionExecutor
            .ExecuteSequentialForLevelAsync(command.LevelId, () => AddTagToLevel(level, tag))
            .ConfigureAwait(false);
    }

    private async Task AddTagToLevel(Level level, Tag tag)
    {
        LevelTag? levelTag = await _DbContext.Set<LevelTag>().FindAsync(level.Id, tag.Id).ConfigureAwait(false);
        if (levelTag != null)
            throw new LevelTagExistsException(level.Id, tag.Name);

        List<LevelTag> conflictingLevelTags = await GetConflictingLevelTagsAsync(level.Id, tag).ConfigureAwait(false);
        LevelTag addedLevelTag = new LevelTag
        {
            Level = level,
            Tag = tag,
        };

        List<ContributionChange> changes = _ContributionChangeManager
            .CreateChanges(tag, conflictingLevelTags.Select(removedLevelTag => removedLevelTag.Tag));
        Contribution contribution = await _ContributionManager
            .CreateContributionAsync(level, tag, ChangeAction.AddTag, changes)
            .ConfigureAwait(false);

        _DbContext.RemoveRange(conflictingLevelTags);
        _DbContext.Add(addedLevelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task<List<LevelTag>> GetConflictingLevelTagsAsync(long levelId, Tag tag)
    {
        if (tag.Exclusivity == null)
            return [];

        List<LevelTag> conflictingLevelTags = await _DbContext.Set<LevelTag>()
            .Include(levelTag => levelTag.Tag)
            .Where(levelTag =>
                levelTag.Level.Id == levelId &&
                levelTag.Tag.GroupId == tag.GroupId &&
                levelTag.Tag.Exclusivity == tag.Exclusivity)
            .ToListAsync()
            .ConfigureAwait(false);

        return conflictingLevelTags;
    }
}