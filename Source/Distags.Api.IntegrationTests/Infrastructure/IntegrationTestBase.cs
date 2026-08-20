using Microsoft.Extensions.Hosting;
using Xunit;

namespace Distags.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Provides API hosts for both environments. The hosts are created per test class rather than per test, because
/// building one is comparatively expensive.
/// </summary>

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncDisposable
{
    private DistagsApiFactory? _DevelopmentApi;
    private DistagsApiFactory? _ProductionApi;

    protected IntegrationTestBase(DatabaseFixture databaseFixture)
    {
        Database = databaseFixture;
    }

    protected DatabaseFixture Database { get; }

    protected DistagsApiFactory DevelopmentApi =>
        _DevelopmentApi ??= new DistagsApiFactory(Database.ApiConnectionString, Environments.Development);

    protected DistagsApiFactory ProductionApi =>
        _ProductionApi ??= new DistagsApiFactory(Database.ApiConnectionString, Environments.Production);

    public virtual async ValueTask DisposeAsync()
    {
        if (_DevelopmentApi != null)
            await _DevelopmentApi.DisposeAsync().ConfigureAwait(false);

        if (_ProductionApi != null)
            await _ProductionApi.DisposeAsync().ConfigureAwait(false);
    }
}
