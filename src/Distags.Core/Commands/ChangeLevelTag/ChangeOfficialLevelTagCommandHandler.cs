using Distags.Core.Entities;
using Distags.Core.Events;
using Distags.Core.Exceptions;
using Distags.Core.Interfaces;
using Distags.Core.Logic;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ChangeLevelTag;

public class ChangeOfficialLevelTagCommandHandler : ChangeLevelTagCommandHandler, ICommandHandler<ChangeOfficialLevelTagCommand>
{
    public ChangeOfficialLevelTagCommandHandler(
        DistagsDbContext dbContext,
        MaintenanceManager maintenanceManager,
        ISequentialLevelActionExecutor sequentialLevelActionExecutor,
        LevelTagAdder levelTagAdder,
        LevelTagRemover levelTagRemover,
        ContributionManager contributionManager,
        AppEventQueue appEventQueue)
        : base(dbContext, maintenanceManager, sequentialLevelActionExecutor, levelTagAdder, levelTagRemover, contributionManager, appEventQueue)
    {
    }

    public async Task HandleAsync(ChangeOfficialLevelTagCommand command)
    {
        long levelId = await GetLevelIdAsync(command.LevelSlug).ConfigureAwait(false);
        await HandleAsync(levelId, command.TagName, command.Action, command.UserSteamId, command.AppName).ConfigureAwait(false);
    }

    private async Task<long> GetLevelIdAsync(string slug)
    {
        long? levelId = await DbContext.Set<OfficialLevel>()
            .Include(level => level.Tags)
            .Where(level => level.Slug == slug)
            .Select(level => level.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        return levelId ?? throw new UnknownLevelException();
    }
}
