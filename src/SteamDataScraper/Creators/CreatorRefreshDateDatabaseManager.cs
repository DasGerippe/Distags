using Distags.Core;
using Distags.Core.Commands.SetLastCreatorRefreshDate;
using Distags.Core.Queries.LastCreatorRefreshDate;

namespace SteamDataScraper.Creators;

public class CreatorRefreshDateDatabaseManager : ICreatorRefreshDateManager
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public CreatorRefreshDateDatabaseManager(IRequestDispatcher requestDispatcher)
    {
        _RequestDispatcher = requestDispatcher;
    }

    public async Task<DateTimeOffset?> GetRefreshDateAsync(CancellationToken cancellationToken = default)
    {
        return await _RequestDispatcher.DispatchAsync(new LastCreatorRefreshDateQuery()).ConfigureAwait(false);
    }

    public async Task SetRefreshDateAsync(DateTimeOffset refreshDate, CancellationToken cancellationToken = default)
    {
        SetLastCreatorRefreshDateCommand command = new SetLastCreatorRefreshDateCommand { LastCreatorRefreshDate = refreshDate };
        await _RequestDispatcher.DispatchAsync(command).ConfigureAwait(false);
    }
}
