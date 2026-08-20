using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DistagsDbContext = Distags.Core.DistagsDbContext;

namespace Distags.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Attention Attention - Tricky connection stringi!
/// The API loads its own user secrets in Development, which hold the real development connection string.
/// If the test redirect ever stops working, the whole suite would silently run against the developer's database,
/// passing for the wrong reasons and corrupting real data.
/// Its a race condition between configuration "change" and dbcontext
/// </summary>
/// <remarks>
/// This asserts on the connection the DbContext actually got, not on the final  <c>IConfiguration</c>.
/// Those two can disagree: <see cref="Program"/> reads the connection string during service registration,
/// so a configuration value applied later shows up in IConfiguration while the DbContext is already bound to the old one.
/// </remarks>
public class ConnectionStringGuardTests : IntegrationTestBase
{
    public ConnectionStringGuardTests(DatabaseFixture databaseFixture)
        : base(databaseFixture)
    {
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Api_Queries_The_Test_Database(bool development)
    {
        DistagsApiFactory api = development ? DevelopmentApi : ProductionApi;

        using IServiceScope scope = api.Services.CreateScope();
        DistagsDbContext dbContext = scope.ServiceProvider.GetRequiredService<DistagsDbContext>();

        dbContext.Database
            .GetConnectionString()
            .Should()
            .Be(Database.ApiConnectionString, "the tests must never touch the developer's database");
    }
}
