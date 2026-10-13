using AutoTagging.Abstractions.TaggingStrategy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AutoTagging.Abstractions.Progress;

public class AutoTaggerProgressFileManager : IAutoTaggerProgressManager
{
    private readonly string _DirectoryPath;
    private readonly ILogger<AutoTaggerProgressFileManager> _Logger;

    private readonly JsonSerializerOptions _JsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public AutoTaggerProgressFileManager(IConfiguration configuration, ILogger<AutoTaggerProgressFileManager> logger)
    {
        _DirectoryPath = configuration["ProgressDirectoryPath"] ?? string.Empty;
        _Logger = logger;
    }

    public async Task<IAutoTaggerProgress?> LoadProgressAsync(AutoTagger autoTagger)
    {
        Type? progressType = autoTagger.TaggingStrategy.GetProgressType();
        if (progressType == null)
            return null;

        string progressFilePath = GetProgressFilePath(autoTagger);
        _Logger.LogDebug("Reading progress from file '{filePath}'...", progressFilePath);

        if (!File.Exists(progressFilePath))
        {
            _Logger.LogDebug("Progress file does not exist.");
            return null;
        }

        try
        {
            FileStream fileStream = new FileStream(progressFilePath, FileMode.Open);
            await using ConfiguredAsyncDisposable _ = fileStream.ConfigureAwait(false);

            IAutoTaggerProgress? progress = await JsonSerializer
                .DeserializeAsync(fileStream, progressType, _JsonSerializerOptions)
                .ConfigureAwait(false) as IAutoTaggerProgress;

            if (progress == null)
                _Logger.LogWarning("Could not deserialize progress into type {progressType}.", progressType.Name);

            return progress;
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "Could not read or deserialize progress file.");
            return null;
        }
    }

    public async Task SaveProgressAsync(AutoTagger autoTagger, IAutoTaggerProgress? progress)
    {
        if (progress == null)
            return;

        string progressFilePath = GetProgressFilePath(autoTagger);
        _Logger.LogDebug("Writing progress to file {filePath} ...", progressFilePath);

        try
        {
            FileStream fileStream = new FileStream(progressFilePath, FileMode.Create, FileAccess.Write);
            await using ConfiguredAsyncDisposable _ = fileStream.ConfigureAwait(false);

            await JsonSerializer.SerializeAsync(fileStream, progress, progress.GetType(), _JsonSerializerOptions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "Could not serialize or write progress to file.");
        }
    }

    private string GetProgressFilePath(AutoTagger autoTagger)
    {
        return Path.Combine(_DirectoryPath, $"{autoTagger.Name}.Progress.json");
    }
}
