namespace SteamDataScraper.Creators;

public interface ICreatorRefreshCountCalculator
{
    Task<int> CalculateCreatorRefreshCountAsync(CancellationToken cancellationToken = default);
}