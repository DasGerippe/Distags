using LevelTagApi.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using SteamWebApi.ApiInterfaces;
using SteamWebApi.Dtos.PublishedFileService;

namespace LevelDataPopulator;

internal class LevelDataPopulator
{
    private const int DistanceAppId = 233610;

    private readonly IPublishedFileService _PublishedFileService;
    private readonly ILogger<LevelDataPopulator> _Logger;
    private readonly HttpClient _HttpClient;

    private readonly string _SteamApiKey;
    private readonly string _LevelTagApi;

    public LevelDataPopulator(
        IConfiguration configuration,
        IPublishedFileService publishedFileService,
        HttpClient httpClient,
        ILogger<LevelDataPopulator> logger)
    {
        _PublishedFileService = publishedFileService;
        _HttpClient = httpClient;
        _Logger = logger;

        _SteamApiKey = configuration[ConfigKeys.SteamApiKey] ??
                       throw new InvalidOperationException($"Missing configuration value: {ConfigKeys.SteamApiKey}");
        _LevelTagApi = configuration[ConfigKeys.LevelTagApiUrl] ??
                       throw new InvalidOperationException($"Missing configuration value: {ConfigKeys.LevelTagApiUrl}");
    }

    public async Task SyncLevelsFromSteamApiAsync(CancellationToken cancellationToken = default)
    {
        string lastLevelSyncEndpointUri = $"{_LevelTagApi}/stats/last-level-sync";
        HttpResponseMessage getLastSyncDateResponse = await _HttpClient.GetAsync(lastLevelSyncEndpointUri, cancellationToken);
        if (!getLastSyncDateResponse.IsSuccessStatusCode)
        {
            _Logger.LogWarning("Received status code {statusCode} from {endpoint}. Cancelling level sync.",
                getLastSyncDateResponse.StatusCode, lastLevelSyncEndpointUri);
            return;
        }

        string responseContent = await getLastSyncDateResponse.Content.ReadAsStringAsync(cancellationToken);
        DateTime? lastSyncDate = string.IsNullOrEmpty(responseContent) ? null : JsonSerializer.Deserialize<DateTime?>(responseContent);

        HashSet<PutLevelDto> levelsToSync = new HashSet<PutLevelDto>(new LevelDtoIdComparer());
        await AddNewLevels(levelsToSync, lastSyncDate);
        if (lastSyncDate != null)
            await AddUpdatedLevels(levelsToSync, lastSyncDate.Value);

        if (levelsToSync.Count == 0)
        {
            _Logger.LogDebug("No new or updated levels to be synced.");
            return;
        }

        _Logger.LogDebug("Sending {count} new and/or updated levels to LevelTagApiUrl.", levelsToSync.Count);

        string levelsEndpointUri = $"{_LevelTagApi}/levels/batch";
        string levelsJson = JsonSerializer.Serialize(levelsToSync);
        HttpContent requestContent = new StringContent(levelsJson, Encoding.UTF8, MediaTypeNames.Application.Json);
        HttpResponseMessage postLevelsResponse = await _HttpClient.PutAsync(levelsEndpointUri, requestContent, cancellationToken);

        _Logger.LogDebug("Received status code {statusCode} from LevelTagApiUrl.", postLevelsResponse.StatusCode);
    }

    private async Task AddNewLevels(ICollection<PutLevelDto> levelsToSync, DateTime? lastSyncDate)
    {
        QueryFileParameters queryParams = new QueryFileParameters
        {
            SteamApiKey = _SteamApiKey,
            AppId = DistanceAppId,
            QueryType = QueryType.RankedByPublicationDate,
            ItemsPerPage = GetItemsPerPage(lastSyncDate),
            ReturnMetadata = true,
        };

        await foreach (PublishedFileDetail workshopFile in _PublishedFileService.QueryFilesAsync(queryParams).ConfigureAwait(false))
        {
            if (lastSyncDate != null && workshopFile.TimeCreated <= lastSyncDate)
            {
                _Logger.LogDebug("Arrived at a level that was created prior to the last sync date. Continuing execution.");
                break;
            }

            PutLevelDto? levelDto = ToLevelDto(workshopFile);
            if (levelDto != null)
                levelsToSync.Add(levelDto);
        }
    }

    private async Task AddUpdatedLevels(ICollection<PutLevelDto> levelsToSync, DateTime lastSyncDate)
    {
        QueryFileParameters queryParams = new QueryFileParameters
        {
            SteamApiKey = _SteamApiKey,
            AppId = DistanceAppId,
            QueryType = QueryType.RankedByLastUpdatedDate,
            ItemsPerPage = GetItemsPerPage(lastSyncDate),
            ReturnMetadata = true,
        };

        await foreach (PublishedFileDetail workshopFile in _PublishedFileService.QueryFilesAsync(queryParams).ConfigureAwait(false))
        {
            if (workshopFile.TimeUpdated <= lastSyncDate)
            {
                _Logger.LogDebug("Arrived at a level that was updated prior to the last sync date. Continuing execution.");
                break;
            }

            PutLevelDto? levelDto = ToLevelDto(workshopFile);
            if (levelDto != null)
                levelsToSync.Add(levelDto);
        }
    }

    private static int GetItemsPerPage(DateTime? lastSyncDate)
    {
        if (lastSyncDate == null)
            return 100;

        TimeSpan? elapsedSinceLastSync = DateTime.UtcNow - lastSyncDate;

        if (elapsedSinceLastSync < TimeSpan.FromDays(1))
            return 10;

        if (elapsedSinceLastSync < TimeSpan.FromDays(30))
            return 25;

        return 50;
    }

    private PutLevelDto? ToLevelDto(PublishedFileDetail workshopFile)
    {
        bool idIsLong = long.TryParse(workshopFile.PublishedFileId, out long id);
        string? name = workshopFile.Title;
        bool creatorIdIsLong = long.TryParse(workshopFile.CreatorId, out long creatorId);
        string? fileName = workshopFile.Filename;
        string? fileUrl = workshopFile.FileUrl;
        string? iconUrl = workshopFile.PreviewUrl;
        DateTime? createdOn = workshopFile.TimeCreated;
        DateTime? updatedOn = workshopFile.TimeUpdated;

        if (!idIsLong || name == null || !creatorIdIsLong || fileName == null || fileUrl == null || 
            iconUrl == null || createdOn == null || updatedOn == null)
        {
            _Logger.LogDebug(
                "Encountered a level where at least one of the required properties wasn't specified correctly. " +
                "({id}, {name}, {creatorId}, {fileName}, {fileUrl}, {iconUrl}, {createdOn}, {updatedOn})",
                workshopFile.PublishedFileId, name, workshopFile.CreatorId, fileName, fileUrl, iconUrl, createdOn, updatedOn);
            return null;
        }

        return new PutLevelDto
        {
            Id = id,
            Name = name,
            CreatorId = creatorId,
            FileName = fileName,
            FileUrl = fileUrl,
            IconUrl = iconUrl,
            CreatedOn = createdOn.Value,
            UpdatedOn = updatedOn.Value,
        };
    }
}