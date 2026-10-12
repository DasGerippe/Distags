using Microsoft.AspNetCore.Mvc;

namespace Distags.Api;

/// <summary>
/// Custom <see cref="ActionResult"/> for returning <see cref="ProblemDetails"/> from an endpoint.
/// </summary>
/// <remarks>
/// Uses the <see cref="IProblemDetailsService"/>/<see cref="ProblemDetailsService"/> and
/// thereby the <see cref="IProblemDetailsWriter"/>/<see cref="DefaultProblemDetailsWriter"/>
/// which enriches the <see cref="ProblemDetails"/> with default values like traceId and
/// custom values (if configured with <see cref="ProblemDetailsOptions"/>).
/// </remarks>
public class ProblemResult : ActionResult
{
    private readonly ProblemDetails _ProblemDetails;

    public ProblemResult(ProblemDetails problemDetails)
    {
        _ProblemDetails = problemDetails;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        HttpContext httpContext = context.HttpContext;
        httpContext.Response.StatusCode = _ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;

        IProblemDetailsService problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = _ProblemDetails,
        });
    }
}
