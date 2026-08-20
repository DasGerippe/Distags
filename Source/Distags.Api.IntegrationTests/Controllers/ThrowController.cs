using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.IntegrationTests.Controllers;

/// <summary>
/// Lives in the test project and is wired into the API under test via an application part, so the
/// production project needs no test-only endpoint.
/// </summary>
[ApiController]
[Route("test/throw")]
public class ThrowController : ControllerBase
{
    internal const string ExceptionMessage = "Deliberate failure from the integration tests.";

    [HttpGet]
    public IActionResult Throw()
    {
        throw new InvalidOperationException(ExceptionMessage);
    }
}
