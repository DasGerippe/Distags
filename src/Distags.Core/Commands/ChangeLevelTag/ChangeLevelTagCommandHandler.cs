using Distags.Contracts;
using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Logic;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ChangeLevelTag;

public abstract class ChangeLevelTagCommandHandler
{
    private readonly MaintenanceManager _MaintenanceManager;
    private readonly ISequentialLevelActionExecutor _SequentialLevelActionExecutor;
    private readonly LevelTagAdder _LevelTagAdder;
    private readonly LevelTagRemover _LevelTagRemover;
    private readonly ContributionManager _ContributionManager;
    private readonly AppEventQueue _AppEventQueue;

    protected DistagsDbContext DbContext { get; }

    protected ChangeLevelTagCommandHandler(
        DistagsDbContext dbContext,
        MaintenanceManager maintenanceManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        LevelTagAdder levelTagAdder,
        LevelTagRemover levelTagRemover,
        ContributionManager contributionManager,
        AppEventQueue appEventQueue)
    {
        DbContext = dbContext;
        _MaintenanceManager = maintenanceManager;
        _SequentialLevelActionExecutor = sequentialLevelActionExecutor;
        _LevelTagAdder = levelTagAdder;
        _LevelTagRemover = levelTagRemover;
        _ContributionManager = contributionManager;
        _AppEventQueue = appEventQueue;
    }

    protected async Task HandleAsync(long levelId, string tagName, TagChangeAction action, long? userSteamId, Guid? autoTaggerGuid)
    {
        Tag tag = await GetTagAsync(tagName).ConfigureAwait(false);
        User? user = await GetUserAsync(userSteamId).ConfigureAwait(false);
        AutoTagger? autoTagger = await GetAutoTaggerAsync(autoTaggerGuid).ConfigureAwait(false);

        await EnsureTagChangeAllowedAsync(user, autoTagger, tag).ConfigureAwait(false);

        Contribution contribution = await _SequentialLevelActionExecutor
            .ExecuteSequentialForLevelAsync(
                levelId,
                () => ChangeLevelTagAsync(levelId, tag, action, user, autoTagger))
            .ConfigureAwait(false);

        EnqueueEvents(contribution);
    }

    private async Task<Level> GetLevelAsync(long levelId)
    {
        Level? level = await DbContext.Set<Level>()
            .Include(level => ((WorkshopLevel)level).Creator)
            .Include(level => level.Tags)
            .Where(level => level.Id == levelId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return level ?? throw new UnknownLevelException();
    }

    private async Task<Tag> GetTagAsync(string tagName)
    {
        Tag? tag = await DbContext.Set<Tag>()
            .Include(tag => tag.LogicGroup!.Tags)
            .FirstOrDefaultAsync(tag => tag.Name == tagName)
            .ConfigureAwait(false);
        return tag ?? throw new UnknownTagException(tagName);
    }

    private async Task<User?> GetUserAsync(long? userSteamId)
    {
        if (userSteamId == null)
            return null;

        User? user = await DbContext.Set<User>()
            .Where(user => user.SteamId == userSteamId)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return user ?? throw new UnknownUserException(userSteamId.Value);
    }

    private async Task<AutoTagger?> GetAutoTaggerAsync(Guid? autoTaggerGuid)
    {
        if (autoTaggerGuid == null)
            return null;

        AutoTagger? autoTagger = await DbContext.Set<AutoTagger>()
            .Where(autoTagger => autoTagger.Guid == autoTaggerGuid)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return autoTagger ?? throw new UnknownAutoTaggerException(autoTaggerGuid.Value);
    }

    private async Task EnsureTagChangeAllowedAsync(User? user, AutoTagger? autoTagger, Tag tag)
    {
        if (user?.IsBanned == true)
            throw new UserBannedException();

        if (user?.AcceptsGuidelines == false)
            throw new UserNotAllowedException();

        if (tag.IsAutoTag && (autoTagger == null || user != null))
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

    private async Task<Contribution> ChangeLevelTagAsync(long levelId, Tag tag, TagChangeAction action, User? user, AutoTagger? autoTagger)
    {
        Level level = await GetLevelAsync(levelId).ConfigureAwait(false);

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
            _ContributionManager.CreateContribution(level, tag, user, autoTagger, action, changeResult.Value);

        DbContext.Add(contribution);
        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        return contribution;
    }

    private void EnqueueEvents(Contribution contribution)
    {
        _AppEventQueue.Enqueue(new ContributionCreatedEvent(contribution));
        _AppEventQueue.Enqueue(new LevelTagsChangedEvent(contribution.Level, contribution.Changes));
    }
}
