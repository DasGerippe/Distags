using AwesomeAssertions;
using Distags.Api.IntegrationTests.Infrastructure;
using Distags.Api.IntegrationTests.TestData;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Distags.Api.IntegrationTests;

// first little smoke test suite
public class SmokeTests : IntegrationTestBase
{
    public SmokeTests(DatabaseFixture databaseFixture)
        : base(databaseFixture)
    {
    }

    [Fact]
    public async Task GetConfiguration_Returns_Ok()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = DevelopmentApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/configuration", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JsonElement configuration = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        configuration.GetProperty("maintenanceActive").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task GetTags_Returns_The_Seeded_Taxonomy()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = DevelopmentApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/tags", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JsonElement tags = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        tags.EnumerateArray().Should().NotBeEmpty("the migration runs the DataSeeder");
    }

    [Fact]
    public async Task GetLevel_Returns_ProblemDetails_When_Unknown()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = DevelopmentApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/levels/999999999", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        JsonElement problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        problemDetails.GetProperty("title").GetString().Should().Be("Level Not Found");
    }

    [Fact]
    public async Task GetLevel_Returns_A_Level_Created_By_The_Test_Data_Builder()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await Database.AddCreatorAsync(id: 4242, name: "Smoke Creator");
        await Database.AddLevelAsync(id: 1234, creatorId: 4242, name: "Smoke Level");

        HttpClient client = DevelopmentApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/levels/1234", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JsonElement level = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        level.GetProperty("name").GetString().Should().Be("Smoke Level");
        level.GetProperty("creatorName").GetString().Should().Be("Smoke Creator");
    }
}
