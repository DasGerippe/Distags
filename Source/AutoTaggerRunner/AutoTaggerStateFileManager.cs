using AutoTagging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AutoTaggerRunner;

public class AutoTaggerStateFileManager : IAutoTaggerStateManager
{
    private readonly ILogger<AutoTaggerStateFileManager> _Logger;

    private readonly JsonSerializerOptions _JsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public AutoTaggerStateFileManager(ILogger<AutoTaggerStateFileManager> logger)
    {
        _Logger = logger;
    }

    public async Task<AutoTaggerState> LoadAutoTaggerStateAsync(AutoTagger autoTagger)
    {
        _Logger.LogDebug("Reading state from file...");

        string stateFile = GetStateFilePath(autoTagger);
        if (!File.Exists(stateFile))
        {
            _Logger.LogDebug("State file does not exist. Returning new state.");
            return AutoTaggerState.New;
        }

        try
        {
            FileStream fileStream = new FileStream(stateFile, FileMode.Open);
            await using ConfiguredAsyncDisposable _ = fileStream.ConfigureAwait(false);

            AutoTaggerState? state = await JsonSerializer
                .DeserializeAsync<AutoTaggerState>(fileStream, _JsonSerializerOptions)
                .ConfigureAwait(false);

            if (state != null)
                return state;

            _Logger.LogDebug("Could not deserialize existing state. Returning new state.");
            return AutoTaggerState.New;
        }
        catch (Exception ex)
        {
            _Logger.LogWarning(ex, "Could not read or deserialize state file. Returning new state.");
            return AutoTaggerState.New;
        }
    }

    public async Task SaveAutoTaggerStateAsync(AutoTagger autoTagger, AutoTaggerState state)
    {
        _Logger.LogDebug("Writing state to file...");

        string stateFile = GetStateFilePath(autoTagger);

        FileStream fileStream = new FileStream(stateFile, FileMode.Create, FileAccess.Write);
        await using ConfiguredAsyncDisposable _ = fileStream.ConfigureAwait(false);

        await JsonSerializer.SerializeAsync(fileStream, state, _JsonSerializerOptions).ConfigureAwait(false);
    }

    private string GetStateFilePath(AutoTagger autoTagger)
    {
        return Path.Combine(Constants.PluginsFolderName, $"{autoTagger.Name}.State.json");
    }
}