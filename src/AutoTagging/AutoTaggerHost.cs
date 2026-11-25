using AppHostUtils;
using AutoTagging.Abstractions;
using Distags.Core;
using Distags.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoTagging;

public class AutoTaggerHost : IAsyncDisposable
{
    private readonly IConfiguration _Configuration;
    private readonly ILoggerFactory _LoggerFactory;

    private readonly Dictionary<Type, ServiceProvider> _AutoTaggerServiceProviders = [];

    public AutoTaggerHost(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _Configuration = configuration;
        _LoggerFactory = loggerFactory;
    }

    public async Task ExecuteAsync(IAutoTagger autoTagger, CancellationToken cancellationToken = default)
    {
        ServiceProvider serviceProvider = GetOrCreateServiceProvider(autoTagger);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await autoTagger.ExecuteAsync(scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
    }

    private ServiceProvider GetOrCreateServiceProvider(IAutoTagger autoTagger)
    {
        Type autoTaggerType = autoTagger.GetType();
        if (_AutoTaggerServiceProviders.TryGetValue(autoTaggerType, out ServiceProvider? serviceProvider))
            return serviceProvider;

        ServiceCollection services = new ServiceCollection();

        services.AddSingleton(_LoggerFactory);
        services.AddLogging();
        services.AddSingleton<IConfiguration>(_Configuration.GetSection($"{nameof(AutoTagging)}:{autoTagger.Name}"));
        services.AddDistagsCore();
        services.AddDistagsPersistence(_Configuration.GetRequiredConnectionString("DistagsDatabase"));

        autoTagger.ConfigureServices(services);

        serviceProvider = services.BuildServiceProvider(validateScopes: true);

        _AutoTaggerServiceProviders[autoTaggerType] = serviceProvider;
        return serviceProvider;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (ServiceProvider serviceProvider in _AutoTaggerServiceProviders.Values)
        {
            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }

        _AutoTaggerServiceProviders.Clear();
    }
}
