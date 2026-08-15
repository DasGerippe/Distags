namespace SteamDataScraper.Creators;

public interface ICreatorRefreshDateManager
{
    Task<DateTimeOffset?> GetRefreshDateAsync(CancellationToken cancellationToken = default);

    Task SetRefreshDateAsync(DateTimeOffset refreshDate, CancellationToken cancellationToken = default);
}
