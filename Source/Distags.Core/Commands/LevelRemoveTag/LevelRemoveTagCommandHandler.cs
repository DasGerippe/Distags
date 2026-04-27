using DataClasses;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.LevelRemoveTag;

public class LevelRemoveTagCommandHandler : ICommandHandler<LevelRemoveTagCommand>
{
    private readonly DbContext _DbContext;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly ContributionManager _ContributionManager;
    private readonly ContributionChangeManager _ContributionChangeManager;

    public LevelRemoveTagCommandHandler(
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

    public async Task HandleAsync(LevelRemoveTagCommand command)
    {
        Level level = await _DbContext.FindAsync<Level>(command.LevelId).ConfigureAwait(false)
                      ?? throw new UnknownLevelException(command.LevelId);
        Tag tag = await _DbContext.Set<Tag>().FirstOrDefaultAsync(tag => tag.Name == command.TagName).ConfigureAwait(false)
                  ?? throw new UnknownTagException(command.TagName);

        await _SequentialLevelActionExecutor
            .ExecuteSequentialForLevelAsync(command.LevelId, () => RemoveTagFromLevel(level, tag))
            .ConfigureAwait(false);
    }

    private async Task RemoveTagFromLevel(Level level, Tag tag)
    {
        LevelTag? levelTag = await _DbContext.FindAsync<LevelTag>(level.Id, tag.Id).ConfigureAwait(false);
        if (levelTag == null)
            throw new LevelTagNotFoundException(level.Id, tag.Name);

        ContributionChange change = _ContributionChangeManager.CreateChange(ChangeAction.RemoveTag, tag);
        Contribution contribution = await _ContributionManager
            .CreateContributionAsync(level, tag, ChangeAction.RemoveTag, [change])
            .ConfigureAwait(false);

        _DbContext.Remove(levelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}