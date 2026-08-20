using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _ProblemDetailsService;
    private readonly IHostEnvironment _Environment;
    private readonly ILogger<GlobalExceptionHandler> _Logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment,
        ILogger<GlobalExceptionHandler> logger)
    {
        _ProblemDetailsService = problemDetailsService;
        _Environment = environment;
        _Logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _Logger.LogError(
            exception,
            "An unhandled exception occurrrrrred while processing {RequestMethod} {RequestPath}.",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        ProblemDetails problemDetails = ProblemDetailsResult.UnexpectedError();

        if (_Environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().FullName;
            problemDetails.Extensions["message"] = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.ToString();
        }

        ProblemDetailsContext problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
        };

        if (await _ProblemDetailsService.TryWriteAsync(problemDetailsContext).ConfigureAwait(false))
            return true;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}
