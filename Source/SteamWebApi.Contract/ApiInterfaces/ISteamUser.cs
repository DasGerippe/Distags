using SteamWebApi.Dtos.SteamUser;

namespace SteamWebApi.ApiInterfaces;

public interface ISteamUser
{
    IAsyncEnumerable<PlayerSummary> GetPlayerSummariesAsync(
        GetPlayerSummariesParameters parameters, CancellationToken cancellationToken = default);
}