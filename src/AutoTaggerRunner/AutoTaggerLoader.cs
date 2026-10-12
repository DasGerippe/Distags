using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AutoTaggerRunner;

public class AutoTaggerLoader
{
    private readonly IServiceProvider _ServiceProvider;
    private readonly ILogger<AutoTaggerLoader> _Logger;

    public AutoTaggerLoader(IServiceProvider serviceProvider, ILogger<AutoTaggerLoader> logger)
    {
        _ServiceProvider = serviceProvider;
        _Logger = logger;
    }

    public List<AutoTagger> LoadAutoTaggersFromDlls(string[] args)
    {
        _Logger.LogDebug("Loading AutoTaggers from .dll's...");

        IEnumerable<string> dllFiles = Directory.EnumerateFiles(Constants.PluginsFolderName)
            .Where(file => file.EndsWith(".dll"))
            .Where(file => args.Length == 0 || args.Any(file.Contains));

        List<AutoTagger> autoTaggers = dllFiles.SelectMany(LoadAutoTaggersFromDll).ToList();

        _Logger.LogDebug("Loaded {count} AutoTaggers.", autoTaggers.Count);

        return autoTaggers;
    }

    private List<AutoTagger> LoadAutoTaggersFromDll(string dllPath)
    {
        _Logger.LogDebug("Loading AutoTaggers from {dllPath}...", dllPath);

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);

            List<AutoTagger> autoTaggers = assembly.GetTypes()
                .Where(type => typeof(AutoTagger).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(pluginType => ActivatorUtilities.CreateInstance(_ServiceProvider, pluginType))
                .OfType<AutoTagger>()
                .ToList();

            _Logger.LogDebug("AutoTaggers found in {dllPath}: {autoTaggerNames}",
                dllPath, string.Join(", ", autoTaggers.Select(autoTagger => autoTagger.Name)));

            return autoTaggers;
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occured while loading AutoTaggers from {dllPath}.", dllPath);
            return [];
        }
    }
}
