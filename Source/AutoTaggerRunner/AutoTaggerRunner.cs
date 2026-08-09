using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AutoTaggerRunner;

public class AutoTaggerRunner
{
    private readonly IAutoTaggerStateManager _StateManager;
    private readonly IAutoTaggerLevelProviderFactory _LevelProviderFactory;
    private readonly DistagsApiClient _DistagsApiClient;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<AutoTaggerRunner> _Logger;

    public AutoTaggerRunner(
        IAutoTaggerStateManager stateManager,
        IAutoTaggerLevelProviderFactory levelProviderFactory,
        DistagsApiClient distagsApiClient,
        IConfiguration configuration,
        ILogger<AutoTaggerRunner> logger)
    {
        _StateManager = stateManager;
        _LevelProviderFactory = levelProviderFactory;
        _DistagsApiClient = distagsApiClient;
        _Configuration = configuration;
        _Logger = logger;
    }

    public async Task RunAutoTaggerPluginsAsync(string[] args)
    {
        _Logger.LogDebug("Starting application...");

        IEnumerable<string> dllFiles = Directory.EnumerateFiles(Constants.PluginsFolderName)
            .Where(file => file.EndsWith(".dll"))
            .Where(file => args.Length == 0 || args.Any(file.Contains));

        foreach (string dllFile in dllFiles)
        {
            await TryRunAutoTaggersAsync(dllFile).ConfigureAwait(false);
        }

        _Logger.LogDebug("End of application.");
    }

    private async Task TryRunAutoTaggersAsync(string pluginDll)
    {
        _Logger.LogDebug("Running {AutoTagger}s in {pluginDll}...", nameof(AutoTagger), pluginDll);

        try
        {
            Assembly assembly = Assembly.LoadFrom(pluginDll);

            List<AutoTagger> autoTaggers = assembly.GetTypes()
                .Where(type => typeof(AutoTagger).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(pluginType => Activator.CreateInstance(pluginType))
                .OfType<AutoTagger>()
                .ToList();

            if (autoTaggers.Count == 0)
            {
                _Logger.LogInformation("No {AutoTagger}s found in {pluginDll}.", nameof(AutoTagger), pluginDll);
                return;
            }

            foreach (AutoTagger autoTagger in autoTaggers)
            {
                await TryRunAutoTaggerAsync(autoTagger).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occured while running {AutoTagger}s of DLL {dll}.", nameof(AutoTagger), pluginDll);
        }
    }

    private async Task TryRunAutoTaggerAsync(AutoTagger autoTagger)
    {
        _Logger.LogDebug("Running {autoTagger}...", autoTagger.Name);

        try
        {
            AutoTaggerState state = await _StateManager.LoadAutoTaggerStateAsync(autoTagger).ConfigureAwait(false);

            await TryRunAutoTaggerAsync(autoTagger, state);

            await _StateManager.SaveAutoTaggerStateAsync(autoTagger, state).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while loading or saving state of {autoTagger}.", autoTagger.Name);
        }
    }

    private async Task TryRunAutoTaggerAsync(AutoTagger autoTagger, AutoTaggerState state)
    {
        try
        {
            IConfiguration autoTaggerConfig = CreateAutoTaggerConfiguration(autoTagger);
            string? distagsApiKey = autoTaggerConfig["DistagsApiKey"];
            if (string.IsNullOrEmpty(distagsApiKey))
            {
                _Logger.LogWarning("Distags API key not configured for {AutoTagger}.", autoTagger.Name);
                return;
            }

            LevelProjectionIncludes levelIncludes = EnrichLevelProjectionIncludes(autoTagger);
            IAutoTaggerLevelProvider levelProvider = _LevelProviderFactory.CreateLevelProvider(autoTagger.TaggingStrategy, levelIncludes);
            List<LevelProjectionDto> levels = await levelProvider.GetLevelsToTagAsync(state).ConfigureAwait(false);

            _Logger.LogDebug("Got {levelCount} levels to tag.", levels.Count);

            foreach (LevelProjectionDto level in levels)
            {
                _Logger.LogDebug("Calculating tag delta for level {levelId} with {autoTagger}...", level.Id, autoTagger.Name);

                TagDelta tagDelta = await autoTagger.CalculateTagDeltaAsync(level).ConfigureAwait(false);

                if (tagDelta.AddTags.Count > 0 || tagDelta.RemoveTags.Count > 0)
                {
                    _Logger.LogDebug("Calculated tag delta: +{tagsToAdd}; -{tagsToRemove}", tagDelta.AddTags, tagDelta.RemoveTags);

                    await SendTagChangesToApiAsync(level.Id!.Value, tagDelta, distagsApiKey).ConfigureAwait(false);
                }
                else
                {
                    _Logger.LogDebug("Calculated tag delta is empty.");
                }

                UpdateAutoTaggerState(state, level, autoTagger.TaggingStrategy);
            }
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while running {autoTagger}.", autoTagger.Name);
        }
    }

    private IConfiguration CreateAutoTaggerConfiguration(AutoTagger autoTagger)
    {
        return new ConfigurationBuilder()
            .AddConfiguration(_Configuration.GetSection($"AutoTaggers:{autoTagger.Name}"))
            .AddJsonFile(Path.Combine(Constants.PluginsFolderName, $"{autoTagger.Name}.appsettings.json"), optional: true)
            .AddEnvironmentVariables($"Distags_{autoTagger.Name}_")
            .Build();
    }

    private LevelProjectionIncludes EnrichLevelProjectionIncludes(AutoTagger autoTagger)
    {
        LevelProjectionIncludes includes = autoTagger.RequiredIncludes | LevelProjectionIncludes.Id;

        switch (autoTagger.TaggingStrategy)
        {
            case TaggingStrategy.NewLevels:
                includes |= LevelProjectionIncludes.CreatedOn;
                break;
            case TaggingStrategy.NewAndUpdatedLevels:
                includes |= LevelProjectionIncludes.UpdatedOn;
                break;
        }

        return includes;
    }

    private async Task SendTagChangesToApiAsync(long levelId, TagDelta tagDelta, string distagsApiKey)
    {
        _Logger.LogDebug("Sending tag updates to API...");

        List<Task> tagChangeTasks = [];

        foreach (string tagToAdd in tagDelta.AddTags)
        {
            Task addTagTask = _DistagsApiClient.PutLevelTagAsync(levelId, tagToAdd, distagsApiKey);
            tagChangeTasks.Add(addTagTask);
        }

        foreach (string tagToRemove in tagDelta.RemoveTags)
        {
            Task removeTagTask = _DistagsApiClient.DeleteLevelTagAsync(levelId, tagToRemove, distagsApiKey);
            tagChangeTasks.Add(removeTagTask);
        }

        await Task.WhenAll(tagChangeTasks).ConfigureAwait(false);
    }

    private void UpdateAutoTaggerState(AutoTaggerState state, LevelProjectionDto level, TaggingStrategy strategy)
    {
        state.LastAutoTagging = strategy switch
        {
            TaggingStrategy.NewLevels => level.CreatedOn!.Value,
            TaggingStrategy.NewAndUpdatedLevels => level.UpdatedOn!.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null),
        };
    }
}