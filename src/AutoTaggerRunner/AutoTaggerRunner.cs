using AutoTagging.Abstractions;
using AutoTagging.Abstractions.Progress;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggerLevels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;

namespace AutoTaggerRunner;

public class AutoTaggerRunner
{
    private readonly IAutoTaggerProgressManager _ProgressManager;
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly DistagsApiClient _DistagsApiClient;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AutoTaggerRunner> _Logger;

    public AutoTaggerRunner(
        IAutoTaggerProgressManager progressManager,
        IRequestDispatcher requestDispatcher,
        DistagsApiClient distagsApiClient,
        IConfiguration configuration,
        ILogger<AutoTaggerRunner> logger)
    {
        _ProgressManager = progressManager;
        _RequestDispatcher = requestDispatcher;
        _DistagsApiClient = distagsApiClient;
        _Configuration = configuration;
        _Logger = logger;
    }

    public async Task TryRunAutoTaggersAsync(List<AutoTagger> autoTaggers)
    {
        foreach (AutoTagger autoTagger in autoTaggers)
        {
            await TryRunAutoTaggerAsync(autoTagger);
        }
    }

    private async Task TryRunAutoTaggerAsync(AutoTagger autoTagger)
    {
        _Logger.LogDebug("Running {autoTagger}...", autoTagger.Name);

        try
        {
            IConfiguration autoTaggerConfig = CreateAutoTaggerConfiguration(autoTagger);
            string? distagsApiKey = autoTaggerConfig["DistagsApiKey"];
            if (string.IsNullOrEmpty(distagsApiKey))
            {
                _Logger.LogWarning("Distags API key not configured for {AutoTagger}.", autoTagger.Name);
                return;
            }

            IAutoTaggerProgress? progress = await _ProgressManager.LoadProgressAsync(autoTagger).ConfigureAwait(false);

            List<LevelDto> levels = await QueryLevels(autoTagger, progress).ConfigureAwait(false);
            progress = await TryRunAutoTaggerAsync(autoTagger, progress, levels, distagsApiKey);

            await _ProgressManager.SaveProgressAsync(autoTagger, progress).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while loading or saving state of {autoTagger}.", autoTagger.Name);
        }
    }

    private async Task<IAutoTaggerProgress?> TryRunAutoTaggerAsync(AutoTagger autoTagger, IAutoTaggerProgress? progress, List<LevelDto> levels, string distagsApiKey)
    {
        try
        {
            _Logger.LogDebug("Got {levelCount} levels to tag.", levels.Count);

            foreach (LevelDto level in levels)
            {
                _Logger.LogDebug("Calculating tag delta for {levelType} level {levelName} with {autoTagger}...",
                    level.Type, level.Name, autoTagger.Name);

                TagDelta tagDelta = await autoTagger.CalculateTagDeltaAsync(level).ConfigureAwait(false);

                if (tagDelta.AddTags.Count > 0 || tagDelta.RemoveTags.Count > 0)
                {
                    _Logger.LogDebug("Calculated tag delta: +{tagsToAdd}; -{tagsToRemove}", tagDelta.AddTags, tagDelta.RemoveTags);

                    await SendTagChangesToApiAsync(level, tagDelta, distagsApiKey).ConfigureAwait(false);
                }
                else
                {
                    _Logger.LogDebug("Calculated tag delta is empty.");
                }

                progress = autoTagger.TaggingStrategy.UpdateProgress(progress, level);
            }
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while running {autoTagger}.", autoTagger.Name);
        }

        return progress;
    }

    private IConfiguration CreateAutoTaggerConfiguration(AutoTagger autoTagger)
    {
        return new ConfigurationBuilder()
            .AddConfiguration(_Configuration.GetSection($"AutoTaggers:{autoTagger.Name}"))
            .AddJsonFile(Path.Combine(Constants.PluginsFolderName, $"{autoTagger.Name}.appsettings.json"), optional: true)
            .AddEnvironmentVariables($"Distags_{autoTagger.Name}_")
            .Build();
    }

    private async Task<List<LevelDto>> QueryLevels(AutoTagger autoTagger, IAutoTaggerProgress? progress)
    {
        AutoTaggerLevelsQuery query = autoTagger.TaggingStrategy.GetLevelsQuery(progress);
        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(query);
        return levels;
    }

    private async Task SendTagChangesToApiAsync(LevelDto level, TagDelta tagDelta, string distagsApiKey)
    {
        _Logger.LogDebug("Sending tag updates to API...");

        foreach (string tagToAdd in tagDelta.AddTags)
        {
            try
            {
                await _DistagsApiClient.PutLevelTagAsync(level, tagToAdd, distagsApiKey).ConfigureAwait(false);
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.Conflict)
            {
                Log.Warning("Could not add tag '{tagName}' to level because it was already present.", tagToAdd);
            }
        }

        foreach (string tagToRemove in tagDelta.RemoveTags)
        {
            try
            {
                await _DistagsApiClient.DeleteLevelTagAsync(level, tagToRemove, distagsApiKey).ConfigureAwait(false);
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Warning("Could not remove tag '{tagName}' from level because it wasn't present.", tagToRemove);
            }
        }
    }
}
