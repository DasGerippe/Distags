using Microsoft.Extensions.Logging;
using System.Globalization;

namespace SteamDataScraper.Creators;

public class CreatorRefreshDateFileManager : ICreatorRefreshDateManager
{
    private const string FileName = "CreatorRefreshDate.txt";

    private readonly ILogger<CreatorRefreshDateFileManager> _Logger;

    public CreatorRefreshDateFileManager(ILogger<CreatorRefreshDateFileManager> logger)
    {
        _Logger = logger;
    }

    public async Task<DateTimeOffset?> GetRefreshDateAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Getting refresh date...");

        if (!File.Exists(FileName))
        {
            _Logger.LogDebug("File {refreshFile} does not exist.", FileName);
            return null;
        }

        string refreshDateString = await File.ReadAllTextAsync(FileName, cancellationToken);
        if (!DateTimeOffset.TryParse(refreshDateString, out DateTimeOffset refreshDate))
        {
            _Logger.LogWarning("Could not parse refresh date from file {refreshFile}.", refreshDate);
            return null;
        }

        _Logger.LogDebug("Refresh date is {refreshDate:O}.", refreshDate);
        return refreshDate;
    }

    public async Task SetRefreshDateAsync(DateTimeOffset refreshDate, CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Setting refresh date to {refreshDate:O}...", refreshDate);
        await File.WriteAllTextAsync(FileName, refreshDate.ToString("O"), cancellationToken);
    }
}