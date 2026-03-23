using DataClasses;
using DataStoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SteamDataScraper.Creators;

public class CreatorRefreshCountCalculator : ICreatorRefreshCountCalculator
{
    private readonly LevelTagDbContext _DbContext;
    private readonly CreatorRefreshSettings _Settings;
    private readonly ICreatorRefreshDateManager _RefreshDateManager;
    private readonly ILogger<CreatorRefreshCountCalculator> _Logger;

    public CreatorRefreshCountCalculator(
        LevelTagDbContext dbContext,
        CreatorRefreshSettings settings,
        ICreatorRefreshDateManager refreshDateManager,
        ILogger<CreatorRefreshCountCalculator>  logger)
    {
        _DbContext = dbContext;
        _Settings = settings;
        _RefreshDateManager = refreshDateManager;
        _Logger = logger;
    }

    public async Task<int> CalculateCreatorRefreshCountAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Calculating creator refresh count...");

        int creatorCount = await _DbContext.Set<Creator>().CountAsync(cancellationToken).ConfigureAwait(false);
        if (creatorCount == 0)
        {
            _Logger.LogDebug("No creators exist that could be refreshed.");
            return 0;
        }

        DateTime? lastRefreshDate = await _RefreshDateManager.GetRefreshDateAsync(cancellationToken);
        if (lastRefreshDate == null)
        {
            _Logger.LogDebug("Previous refresh date does not exist. Setting initial refresh date and returning 0.");
            await _RefreshDateManager.SetRefreshDateAsync(DateTime.UtcNow, cancellationToken);
            return 0;
        }

        double refreshPercentage = (DateTime.UtcNow - lastRefreshDate.Value).TotalDays / _Settings.RefreshCycleDays;
        int refreshCount = (int)Math.Round(refreshPercentage * creatorCount);

        if (refreshCount > 0 && refreshCount < _Settings.MinRefreshCount)
        {
            _Logger.LogDebug("Calculated refresh count ({refreshCount}) is below threshold ({threshold}). Return 0 instead.",
                refreshCount, _Settings.MinRefreshCount);
            return 0;
        }

        _Logger.LogDebug("Calculated refresh count is {refreshCount}.", refreshCount);
        return refreshCount;
    }
}