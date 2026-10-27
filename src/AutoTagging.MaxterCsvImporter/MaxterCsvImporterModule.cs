using AutoTagging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporterModule : IAutoTaggerModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.TryAddTransient<MaxterCsvRepositoryFactory>();
        services.TryAddTransient<MaxterTagMappingLoader>();
        services.TryAddTransient<MaxterTagResolver>();
    }
}
