using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ChangeLevelTag;

public class ChangeLevelTagCommandHandler : ICommandHandler<ChangeLevelTagCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly MaintenanceManager _MaintenanceManager;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly ContributionManager _ContributionManager;
    private readonly ContributionChangeManager _ContributionChangeManager;
    private readonly AppEventQueue _AppEventQueue;

    public ChangeLevelTagCommandHandler(
        DistagsDbContext dbContext,
        MaintenanceManager maintenanceManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        ContributionManager contributionManager,
        ContributionChangeManager contributionChangeManager,
        AppEventQueue appEventQueue)
    {
        _DbContext = dbContext;
        _MaintenanceManager = maintenanceManager;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _ContributionManager = contributionManager;
        _ContributionChangeManager = contributionChangeManager;
        _AppEventQueue = appEventQueue;
    }

    public async Task HandleAsync(ChangeLevelTagCommand command)
    {
        Level level = await GetLevelAsync(command.LevelId).ConfigureAwait(false);
        Tag tag = await GetTagAsync(command.TagName).ConfigureAwait(false);
        User? user = await GetUserAsync(command.UserSteamId).ConfigureAwait(false);
        App? app = await GetAppAsync(command.AppName).ConfigureAwait(false);

        await EnsureTagChangeAllowedAsync(user, app, tag).ConfigureAwait(false);

        Func<Task<Contribution>> changeFunc = command.ChangeAction == ChangeActionDto.AddTag
            ? () => AddTagToLevel(level, tag, user, app)
            : () => RemoveTagFromLevel(level, tag, user, app);

        Contribution contribution = await _SequentialLevelActionExecutor
            .ExecuteSequentialForLevelAsync(command.LevelId, changeFunc)
            .ConfigureAwait(false);

        EnqueueEvents(contribution);
    }

    private async Task<Level> GetLevelAsync(long levelId)
    {
        Level? level = await _DbContext.FindAsync<Level>(levelId).ConfigureAwait(false);
        return level ?? throw new UnknownLevelException(levelId);
    }

    private async Task<Tag> GetTagAsync(string tagName)
    {
        Tag? tag = await _DbContext.Set<Tag>().FirstOrDefaultAsync(tag => tag.Name == tagName).ConfigureAwait(false);
        return tag ?? throw new UnknownTagException(tagName);
    }

    private async Task<User?> GetUserAsync(long? userSteamId)
    {
        if (userSteamId == null)
            return null;

        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == userSteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return user ?? throw new UnknownUserException(userSteamId.Value);
    }

    private async Task<App?> GetAppAsync(string? appName)
    {
        if (appName == null)
            return null;

        App? app = await _DbContext.Set<App>()
            .Where(app => app.Name == appName)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return app ?? throw new UnknownAppNameException(appName);
    }

    private async Task EnsureTagChangeAllowedAsync(User? user, App? app, Tag tag)
    {
        if (tag.IsAutoTag && (app == null || user != null))
            throw new CannotChangeAutoTagException();

        if (user != null && !user.Roles.Contains("Administrator") && !user.Roles.Contains("Moderator"))
        {
            bool isMaintenanceActive = await _MaintenanceManager.GetSettingValueAsync().ConfigureAwait(false);
            if (isMaintenanceActive)
                throw new MaintenanceActiveException();
        }
    }

    private async Task<Contribution> AddTagToLevel(Level level, Tag tag, User? user, App? app)
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
        Contribution contribution = _ContributionManager
            .CreateContribution(level, tag, user, app, ChangeAction.AddTag, changes);

        _DbContext.RemoveRange(conflictingLevelTags);
        _DbContext.Add(addedLevelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
        return contribution;
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

    private async Task<Contribution> RemoveTagFromLevel(Level level, Tag tag, User? user, App? app)
    {
        LevelTag? levelTag = await _DbContext.FindAsync<LevelTag>(level.Id, tag.Id).ConfigureAwait(false);
        if (levelTag == null)
            throw new LevelTagNotFoundException(level.Id, tag.Name);

        ContributionChange change = _ContributionChangeManager.CreateChange(ChangeAction.RemoveTag, tag);
        Contribution contribution = _ContributionManager
            .CreateContribution(level, tag, user, app, ChangeAction.RemoveTag, [change]);

        _DbContext.Remove(levelTag);
        _DbContext.Add(contribution);

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
        return contribution;
    }

    private void EnqueueEvents(Contribution contribution)
    {
        _AppEventQueue.Enqueue(new ContributionCreatedEvent(contribution.Id));

        List<TagChangeDto> changeDtos = contribution.Changes
            .Select(change => new TagChangeDto
            {
                ChangeType = (TagChangeType)change.Action,
                TagName = change.TagName,
            })
            .ToList();
        _AppEventQueue.Enqueue(new LevelTagsChangedEvent(contribution.LevelId, changeDtos));
    }
}