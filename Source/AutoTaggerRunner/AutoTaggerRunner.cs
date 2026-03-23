using AutoTagging.Abstractions;
using DataClasses;
using DataStoring;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AutoTaggerRunner;

public class AutoTaggerRunner
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IAutoTaggerStateManager _StateManager;
    private readonly IAutoTaggerStateUpdater _StateUpdater;
    private readonly IAutoTaggerLevelProviderFactory _LevelProviderFactory;
    private readonly LevelTagApiClient _LevelTagApi;
    private readonly ILogger<AutoTaggerRunner> _Logger;

    public AutoTaggerRunner(
        LevelTagDbContext dbContext,
        IAutoTaggerStateManager stateManager,
        IAutoTaggerStateUpdater stateUpdater,
        IAutoTaggerLevelProviderFactory levelProviderFactory,
        LevelTagApiClient levelTagApi,
        ILogger<AutoTaggerRunner> logger)
    {
        _DbContext = dbContext;
        _StateManager = stateManager;
        _StateUpdater = stateUpdater;
        _LevelProviderFactory = levelProviderFactory;
        _LevelTagApi = levelTagApi;
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
            _Logger.LogError(ex, null);
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
            IAutoTaggerLevelProvider levelProvider = _LevelProviderFactory.CreateLevelProvider(autoTagger.TaggingStrategy);
            List<Level> levels = await levelProvider.GetLevelsToTagAsync(state).ConfigureAwait(false);

            _Logger.LogDebug("Got {levelCount} levels to tag.", levels.Count);

            foreach (Level level in levels)
            {
                _Logger.LogDebug("Calculating tag delta for level {levelId} with {autoTagger}...", level.Id, autoTagger.Name);

                TagDelta tagDelta = await autoTagger.CalculateTagDeltaAsync(level).ConfigureAwait(false);

                if (tagDelta.AddTags.Any() || tagDelta.RemoveTags.Any())
                {
                    _Logger.LogDebug("Calculated tag delta: +{tagsToAdd}; -{tagsToRemove}", tagDelta.AddTags, tagDelta.RemoveTags);

                    await SendTagChangesToApiAsync(level.Id, tagDelta).ConfigureAwait(false);
                }
                else
                {
                    _Logger.LogDebug("Calculated tag delta is empty.");
                }

                _StateUpdater.UpdateAutoTaggerState(state, level, autoTagger.TaggingStrategy);
            }
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while running {autoTagger}.", autoTagger.Name);
        }
    }

    private async Task SendTagChangesToApiAsync(long levelId, TagDelta tagDelta)
    {
        _Logger.LogDebug("Sending tag updates to API...");

        List<Task> tagChangeTasks = [];

        foreach (string tagToAdd in tagDelta.AddTags)
        {
            Task postLevelTagTask = _LevelTagApi.PostLevelTagAsync(levelId, tagToAdd);
            tagChangeTasks.Add(postLevelTagTask);
        }

        foreach (string tagToRemove in tagDelta.RemoveTags)
        {
            Task deleteLevelTagTask = _LevelTagApi.DeleteLevelTagAsync(levelId, tagToRemove);
            tagChangeTasks.Add(deleteLevelTagTask);
        }

        await Task.WhenAll(tagChangeTasks).ConfigureAwait(false);
    }
}