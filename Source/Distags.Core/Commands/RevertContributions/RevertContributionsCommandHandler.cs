using Distags.Core.Entities;
using Distags.Core.Management.History;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.RevertContributions;

public class RevertContributionsCommandHandler : ICommandHandler<RevertContributionsCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly ContributionReplayEngine _ContributionReplayEngine;

    public RevertContributionsCommandHandler(DistagsDbContext dbContext, ContributionReplayEngine contributionReplayEngine)
    {
        _DbContext = dbContext;
        _ContributionReplayEngine = contributionReplayEngine;
    }

    public async Task HandleAsync(RevertContributionsCommand command)
    {
        DateTimeOffset revertStartDate = command.RevertStartDate ?? DateTimeOffset.MinValue;
        if (revertStartDate.Offset != TimeSpan.Zero)
            revertStartDate = revertStartDate.ToOffset(TimeSpan.Zero);

        List<Contribution> contributionsToRevert = await _DbContext.Set<Contribution>()
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.User!.SteamId == command.UserSteamId &&
                contribution.ContributedOn >= revertStartDate)
            .ToListAsync()
            .ConfigureAwait(false);

        if (contributionsToRevert.Count == 0)
            return;

        foreach (Contribution contribution in contributionsToRevert)
        {
            contribution.IsReverted = true;
        }
        await _DbContext.SaveChangesAsync().ConfigureAwait(false);

        IEnumerable<long> levelIdsToReplay = contributionsToRevert.Select(contribution => contribution.LevelId).Distinct();
        await ReplayContributionsAsync(levelIdsToReplay).ConfigureAwait(false);
    }

    private async Task ReplayContributionsAsync(IEnumerable<long> levelIds)
    {
        foreach (long levelId in levelIds)
        {
            try
            {
                await _ContributionReplayEngine.ReplayContributionsAsync(levelId).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}