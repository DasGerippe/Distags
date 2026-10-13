using Distags.Core;
using Distags.Core.Queries.CreatorCount;
using Microsoft.Extensions.Logging;

namespace SteamDataScraper.Creators;

public class CreatorRefreshCountCalculator : ICreatorRefreshCountCalculator
{
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly CreatorRefreshSettings _Settings;
    private readonly ICreatorRefreshDateManager _RefreshDateManager;
    private readonly ILogger<CreatorRefreshCountCalculator> _Logger;

    public CreatorRefreshCountCalculator(
        IRequestDispatcher requestDispatcher,
        CreatorRefreshSettings settings,
        ICreatorRefreshDateManager refreshDateManager,
        ILogger<CreatorRefreshCountCalculator> logger)
    {
        _RequestDispatcher = requestDispatcher;
        _Settings = settings;
        _RefreshDateManager = refreshDateManager;
        _Logger = logger;
    }

    public async Task<int> CalculateCreatorRefreshCountAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Calculating creator refresh count...");

        long creatorCount = await _RequestDispatcher.DispatchAsync(new CreatorCountQuery()).ConfigureAwait(false);
        if (creatorCount == 0)
        {
            _Logger.LogDebug("No creators exist that could be refreshed.");
            return 0;
        }

        DateTimeOffset? lastRefreshDate = await _RefreshDateManager.GetRefreshDateAsync(cancellationToken);
        if (lastRefreshDate == null)
        {
            _Logger.LogDebug("Previous refresh date does not exist. Setting initial refresh date and returning 0.");
            await _RefreshDateManager.SetRefreshDateAsync(DateTimeOffset.UtcNow, cancellationToken);
            return 0;
        }

        double daysSinceLastRefresh = (DateTimeOffset.UtcNow - lastRefreshDate.Value).TotalDays;
        double refreshPercentage = Math.Min(daysSinceLastRefresh / _Settings.RefreshCycleDays, 1.0);
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
