using Microsoft.AspNetCore.Mvc;

namespace Distags.WebApp.DistagsApi;

public class ApiException : Exception
{
    public ProblemDetails? ProblemDetails { get; init; }

    public ApiException()
    {
    }

    public ApiException(ProblemDetails? problemDetails)
    {
        ProblemDetails = problemDetails;
    }
}
