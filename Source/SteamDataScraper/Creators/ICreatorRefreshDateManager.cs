namespace SteamDataScraper.Creators;

public interface ICreatorRefreshDateManager
{
    Task<DateTime?> GetRefreshDateAsync(CancellationToken cancellationToken = default);

    Task SetRefreshDateAsync(DateTime refreshDate, CancellationToken cancellationToken = default);
}