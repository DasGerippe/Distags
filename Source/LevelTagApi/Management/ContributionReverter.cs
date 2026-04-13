using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Management;

public class ContributionReverter : IContributionReverter
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionReplayEngine _ContributionReplayEngine;

    public ContributionReverter(LevelTagDbContext dbContext, IContributionReplayEngine contributionReplayEngine)
    {
        _DbContext = dbContext;
        _ContributionReplayEngine = contributionReplayEngine;
    }

    public async Task RevertContributionsAsync(long steamId, DateTimeOffset revertStartDate)
    {
        if (revertStartDate.Offset != TimeSpan.Zero)
            revertStartDate = revertStartDate.ToOffset(TimeSpan.Zero);

        List<Contribution> contributionsToRevert = await _DbContext.Set<Contribution>()
            .Where(contribution =>
                !contribution.IsReverted &&
                contribution.User!.SteamId == steamId &&
                contribution.ContributedOn >= revertStartDate)
            .OrderBy(contribution => contribution.ContributedOn)
            .ToListAsync();

        if (contributionsToRevert.Count == 0)
            return;

        foreach (Contribution contribution in contributionsToRevert)
        {
            contribution.IsReverted = true;
        }
        await _DbContext.SaveChangesAsync();

        IEnumerable<long> levelIdsToReplay = contributionsToRevert.Select(contribution => contribution.LevelId).Distinct();
        await ReplayContributionsAsync(levelIdsToReplay);
    }

    private async Task ReplayContributionsAsync(IEnumerable<long> levelIds)
    {
        foreach (long levelId in levelIds)
        {
            try
            {
                await _ContributionReplayEngine.ReplayContributionsAsync(levelId);
            }
            catch
            {
                // ignored for now
            }
        }
    }
}