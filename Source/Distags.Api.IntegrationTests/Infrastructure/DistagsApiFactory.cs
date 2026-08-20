using Distags.Api.IntegrationTests.Controllers;
using Distags.Api.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Distags.Api.IntegrationTests.Infrastructure;

/// <summary>Hosts <see cref="Program"/> in-process, pointed at the test database.</summary>
public sealed class DistagsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _ConnectionString;
    private readonly string _EnvironmentName;

    public DistagsApiFactory(string connectionString, string environmentName)
    {
        _ConnectionString = connectionString;
        _EnvironmentName = environmentName;
    }

    public CapturingLoggerProvider LoggerProvider { get; } = new CapturingLoggerProvider();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(_EnvironmentName);

        // Added last so it beats the API's own appsettings and - in Development - its user secrets,
        // which would otherwise point the tests at the real development database.
        builder.ConfigureAppConfiguration(configurationBuilder => configurationBuilder
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DistagsDatabase"] = _ConnectionString,
            }));

        builder.ConfigureTestServices(services =>
        {
            RemoveSessionCleanupService(services);
            ReplaceLoggingWithCapturingProvider(services);
            services.AddControllers().AddApplicationPart(typeof(ThrowController).Assembly);
        });
    }

    /// <summary>
    /// Flaky as shit with respawn, respawn wins.
    /// </summary>
    private static void RemoveSessionCleanupService(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(service =>
            service.ServiceType == typeof(IHostedService) &&
            service.ImplementationType == typeof(SessionCleanupService));

        if (descriptor != null)
            services.Remove(descriptor);
    }
    private void ReplaceLoggingWithCapturingProvider(IServiceCollection services)
    {
        services.RemoveAll<ILoggerFactory>();
        services.RemoveAll<ILoggerProvider>();

        services.AddSingleton<ILoggerProvider>(LoggerProvider);
        services.AddLogging();
    }
}
