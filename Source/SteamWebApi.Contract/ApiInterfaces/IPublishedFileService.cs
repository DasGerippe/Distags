using SteamWebApi.Dtos.PublishedFileService;

namespace SteamWebApi.ApiInterfaces;

public interface IPublishedFileService
{
    IAsyncEnumerable<PublishedFileDetail> QueryFilesAsync(
        QueryFileParameters queryParameters, CancellationToken cancellationToken = default);
}