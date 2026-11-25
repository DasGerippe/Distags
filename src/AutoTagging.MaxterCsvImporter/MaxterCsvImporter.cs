using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporter : StrategyAutoTagger
{
    public override Guid Guid { get; } = new Guid("7BD71F81-FF4E-4D41-9A3D-510B82E3E7AC");

    public override TimeSpan? Interval => null;

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddTransient<ITaggingStrategy, MaxterCsvImporterStrategy>();

        services.AddTransient<MaxterCsvRepositoryFactory>();
        services.AddTransient<MaxterTagMappingLoader>();
        services.AddTransient<MaxterTagResolver>();
        services.AddTransient<ImportTagDeltaCalculator>();
    }
}
