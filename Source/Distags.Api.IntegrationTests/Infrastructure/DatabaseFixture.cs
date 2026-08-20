using Distags.Core;
using Distags.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;
using Xunit;

// Distags.Core and Distags.Persistence both define a DistagsDbContext. The abstract one from Core is
// what AddDistagsPersistence registers as the service type, matching Distags.Persistence.Migrator.
using DistagsDbContext = Distags.Core.DistagsDbContext;

namespace Distags.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Owns the database the whole test run shares: either a configured Postgres instance or, if none is  configured,
/// a throwaway container. Applies the migrations from Distags.Persistence.Dev and
/// empties the data once, at the start of the run.
/// </summary>
/// <remarks>
/// The throwaway container is gone once the run ends.
/// </remarks>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private const string ContainerImage = "postgres:17-alpine";

    /// <summary>
    /// Respawn empties every table it is not told to keep. The taxonomy is seeded once by the
    /// migration and kept, so it never has to be re-seeded from the test project.
    /// </summary>
    private static readonly Table[] _KeptTables =
    [
        new Table("public", "__EFMigrationsHistory"),
        new Table("public", "display_groups"),
        new Table("public", "logic_groups"),
        new Table("public", "tags"),
    ];

    private PostgreSqlContainer? _Container;
    private ServiceProvider? _AdminServices;
    private Respawner? _Respawner;

    /// <summary>Connection with schema and write permissions. Used for migrations and test data.</summary>
    public string AdminConnectionString { get; private set; } = string.Empty;

    /// <summary>Connection handed to the API under test. May be a more restricted login.</summary>
    public string ApiConnectionString { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Tests.json", optional: false)
            .Build();

        string? configuredConnectionString = configuration.GetConnectionString("DistagsTestDatabase");

        if (string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            _Container = new PostgreSqlBuilder(ContainerImage).Build();
            await _Container.StartAsync();

            AdminConnectionString = _Container.GetConnectionString();
            ApiConnectionString = AdminConnectionString;
        }
        else
        {
            EnsureDatabaseLooksLikeATestDatabase(configuredConnectionString);

            AdminConnectionString = configuredConnectionString;

            string? configuredApiConnectionString = configuration.GetConnectionString("DistagsTestApiDatabase");
            ApiConnectionString = string.IsNullOrWhiteSpace(configuredApiConnectionString)
                ? configuredConnectionString
                : configuredApiConnectionString;
        }

        RedirectApiConnectionString(ApiConnectionString);

        _AdminServices = BuildAdminServices(AdminConnectionString);

        await MigrateAsync();
        await CreateRespawnerAsync();
        await ResetAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_AdminServices != null)
            await _AdminServices.DisposeAsync().ConfigureAwait(false);

        if (_Container != null)
            await _Container.DisposeAsync().ConfigureAwait(false);
    }

    public async Task ResetAsync()
    {
        await using NpgsqlConnection connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();

        await _Respawner!.ResetAsync(connection);
    }

    public async Task WithDbContextAsync(Func<DistagsDbContext, Task> action)
    {
        await using AsyncServiceScope scope = _AdminServices!.CreateAsyncScope();
        DistagsDbContext dbContext = scope.ServiceProvider.GetRequiredService<DistagsDbContext>();

        await action(dbContext);
    }

    /// <summary>
    /// Points the API under test at the test database, via an environment variable rather than a WebApplicationFactory
    /// configuration override.
    /// </summary>
    private static void RedirectApiConnectionString(string connectionString)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__DistagsDatabase", connectionString);
    }

    private static void EnsureDatabaseLooksLikeATestDatabase(string connectionString)
    {
        string database = new NpgsqlConnectionStringBuilder(connectionString).Database ?? string.Empty;
        if (database.Contains("_test", StringComparison.OrdinalIgnoreCase))
            return;

        throw new InvalidOperationException(
            $"""
            Refusing to run the integration tests against the database '{database}'.
            These tests DELETE all data in the configured database.
            Point 'ConnectionStrings:DistagsTestDatabase' in appsettings.Tests.json at a dedicated test
            database whose name contains '_test', or leave it empty to use a throwaway container.
            """);
    }

    private static ServiceProvider BuildAdminServices(string connectionString)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddDistagsPersistence(
            connectionString,
            npgsqlOptionsAction: npgsqlOptions => npgsqlOptions
                .MigrationsAssembly($"{nameof(Distags)}.{nameof(Persistence)}.Dev"));
        services.AddDistagsCore();
        return services.BuildServiceProvider();
    }

    private async Task MigrateAsync()
    {
        await WithDbContextAsync(dbContext => dbContext.Database.MigrateAsync());
    }

    private async Task CreateRespawnerAsync()
    {
        await using NpgsqlConnection connection = new NpgsqlConnection(AdminConnectionString);
        await connection.OpenAsync();

        _Respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore = _KeptTables,
            });
    }
}
