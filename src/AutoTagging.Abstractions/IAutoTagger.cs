using Microsoft.Extensions.DependencyInjection;

namespace AutoTagging.Abstractions;

public interface IAutoTagger
{
    Guid Guid { get; }

    string Name { get; }

    TimeSpan? Interval { get; }

    void ConfigureServices(IServiceCollection services);

    Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
