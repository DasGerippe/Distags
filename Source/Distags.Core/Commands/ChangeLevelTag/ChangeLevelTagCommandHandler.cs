using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Logic;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ChangeLevelTag;

public class ChangeLevelTagCommandHandler : ICommandHandler<ChangeLevelTagCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly MaintenanceManager _MaintenanceManager;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly LevelTagAdder _LevelTagAdder;
    private readonly LevelTagRemover _LevelTagRemover;
    private readonly ContributionManager _ContributionManager;
    private readonly TagChangeManager _TagChangeManager;
    private readonly AppEventQueue _AppEventQueue;

    public ChangeLevelTagCommandHandler(
        DistagsDbContext dbContext,
        MaintenanceManager maintenanceManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        LevelTagAdder levelTagAdder,
        LevelTagRemover levelTagRemover,
        ContributionManager contributionManager,
        TagChangeManager tagChangeManager,
        AppEventQueue appEventQueue)
    {
        _DbContext = dbContext;
        _MaintenanceManager = maintenanceManager;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _LevelTagAdder = levelTagAdder;
        _LevelTagRemover = levelTagRemover;
        _ContributionManager = contributionManager;
        _TagChangeManager = tagChangeManager;
        _AppEventQueue = appEventQueue;
    }

    public async Task HandleAsync(ChangeLevelTagCommand command)
    {
        Level level = await GetLevelAsync(command.LevelId).ConfigureAwait(false);
        Tag tag = await GetTagAsync(command.TagName).ConfigureAwait(false);
        User? user = await GetUserAsync(command.UserSteamId).ConfigureAwait(false);
        App? app = await GetAppAsync(command.AppName).ConfigureAwait(false);

        await EnsureTagChangeAllowedAsync(user, app, tag).ConfigureAwait(false);

        Contribution contribution = await _SequentialLevelActionExecutor
            .ExecuteSequentialForLevelAsync(command.LevelId, () => ChangeLevelTagAsync(level, tag, command.Action, user, app))
            .ConfigureAwait(false);

        EnqueueEvents(contribution);
    }

    private async Task<Level> GetLevelAsync(long levelId)
    {
        Level? level = await _DbContext.Set<Level>()
            .Include(level => level.Tags)
            .Where(level => level.Id == levelId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return level ?? throw new UnknownLevelException(levelId);
    }

    private async Task<Tag> GetTagAsync(string tagName)
    {
        Tag? tag = await _DbContext.Set<Tag>()
            .Include(tag => tag.LogicGroup!.Tags)
            .FirstOrDefaultAsync(tag => tag.Name == tagName)
            .ConfigureAwait(false);
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
        if (user?.IsBanned == true)
            throw new UserBannedException();

        if (user?.AcceptsGuidelines == false)
            throw new UserNotAllowedException();

        if (tag.IsAutoTag && (app == null || user != null))
            throw new CannotChangeAutoTagException();

        bool isMaintenanceAffectedUser =
            user != null &&
            !user.Roles.Contains(Constants.UserRoles.Administrator) &&
            !user.Roles.Contains(Constants.UserRoles.Moderator);
        if (isMaintenanceAffectedUser)
        {
            bool isMaintenanceActive = await _MaintenanceManager.GetSettingValueAsync().ConfigureAwait(false);
            if (isMaintenanceActive)
                throw new MaintenanceActiveException();
        }
    }

    private async Task<Contribution> ChangeLevelTagAsync(Level level, Tag tag, TagChangeAction action, User? user, App? app)
    {
        DomainResult<List<TagChange>> changeResult = action == TagChangeAction.Add
            ? _LevelTagAdder.AddTagToLevel(level, tag)
            : _LevelTagRemover.RemoveTagFromLevel(level, tag);

        if (!changeResult.IsSuccess)
        {
            switch (changeResult.Error)
            {
                case DomainError.LevelTagAlreadyExists:
                    throw new LevelTagExistsException(level.Id, tag.Name);
                case DomainError.LevelTagDoesNotExist:
                    throw new LevelTagNotFoundException(level.Id, tag.Name);
                case DomainError.CannotRemoveFallbackTag:
                    throw new CannotRemoveFallbackTagException();
                default:
                    throw new Exception();
            }
        }

        Contribution contribution =
            _ContributionManager.CreateContribution(level, tag, user, app, action, changeResult.Value);

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
                Action = change.Action,
                TagName = change.TagName,
            })
            .ToList();
        _AppEventQueue.Enqueue(new LevelTagsChangedEvent(contribution.LevelId, changeDtos));
    }
}