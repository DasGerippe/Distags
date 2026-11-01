using Distags.Core.Management;

namespace Distags.Core.Queries.LastCreatorRefreshDate;

public class LastCreatorRefreshDateQueryHandler : IQueryHandler<LastCreatorRefreshDateQuery, DateTimeOffset?>
{
    private readonly LastCreatorRefreshDateManager _LastCreatorRefreshDateManager;

    public LastCreatorRefreshDateQueryHandler(LastCreatorRefreshDateManager lastCreatorRefreshDateManager)
    {
        _LastCreatorRefreshDateManager = lastCreatorRefreshDateManager;
    }

    public async Task<DateTimeOffset?> HandleAsync(LastCreatorRefreshDateQuery query)
    {
        return await _LastCreatorRefreshDateManager.GetSettingValueAsync().ConfigureAwait(false);
    }
}
