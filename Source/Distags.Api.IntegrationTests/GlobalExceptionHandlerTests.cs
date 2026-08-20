using AwesomeAssertions;
using Distags.Api.IntegrationTests.Controllers;
using Distags.Api.IntegrationTests.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Distags.Api.IntegrationTests;

/// <summary>
/// Guards the promise that unhandled exceptions become a structured ProblemDetails response,
/// they are logged in full, and their details never leave Development.
/// </summary>
public class GlobalExceptionHandlerTests : IntegrationTestBase
{
    private const string ThrowingEndpoint = "/test/throw";

    public GlobalExceptionHandlerTests(DatabaseFixture databaseFixture)
        : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Unhandled_Exception_Returns_ProblemDetails()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = ProductionApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync(ThrowingEndpoint, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        JsonElement problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        problemDetails.GetProperty("title").GetString().Should().Be("Unexpected Error");
        problemDetails.GetProperty("detail").GetString().Should().Be("An unexpected error occurred.");
        problemDetails.GetProperty("status").GetInt32().Should().Be(500);
        problemDetails.GetProperty("traceId").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Unhandled_Exception_Leaks_No_Details_Outside_Development()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = ProductionApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync(ThrowingEndpoint, cancellationToken);
        JsonElement problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        problemDetails.TryGetProperty("exception", out _)
            .Should().BeFalse("exception details must never leave Development");
        problemDetails.TryGetProperty("message", out _)
            .Should().BeFalse("exception details must never leave Development");
        problemDetails.TryGetProperty("stackTrace", out _)
            .Should().BeFalse("exception details must never leave Development");

        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        body.Should().NotContain(ThrowController.ExceptionMessage)
            .And.NotContain(nameof(InvalidOperationException));
    }

    [Fact]
    public async Task Unhandled_Exception_Includes_Details_In_Development()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        HttpClient client = DevelopmentApi.CreateClient();

        HttpResponseMessage response = await client.GetAsync(ThrowingEndpoint, cancellationToken);
        JsonElement problemDetails = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        problemDetails.GetProperty("exception").GetString().Should().Be(typeof(InvalidOperationException).FullName);
        problemDetails.GetProperty("message").GetString().Should().Be(ThrowController.ExceptionMessage);
        problemDetails.GetProperty("stackTrace").GetString().Should().Contain(nameof(ThrowController));
    }

    [Fact]
    public async Task Unhandled_Exception_Is_Logged_With_Its_Stack_Trace()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DistagsApiFactory api = ProductionApi;
        HttpClient client = api.CreateClient();

        await client.GetAsync(ThrowingEndpoint, cancellationToken);

        CapturedLogEntry entry = api.LoggerProvider.Entries.Should()
            .ContainSingle(logEntry => logEntry.CategoryName == typeof(GlobalExceptionHandler).FullName)
            .Which;

        entry.LogLevel.Should().Be(LogLevel.Error);
        entry.Message.Should().Contain(ThrowingEndpoint);
        entry.Exception.Should().BeOfType<InvalidOperationException>()
            .Which.Message.Should().Be(ThrowController.ExceptionMessage);
    }
}
