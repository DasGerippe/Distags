using Microsoft.AspNetCore.Mvc;

namespace Distags.Api;

public static class ProblemDetailsResult
{
    public static ObjectResult UserNotFound(long userSteamId)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "User Not Found",
            Detail = $"Could not find user with Steam ID {userSteamId}.",
        });
    }

    private static ObjectResult ObjectResult(ProblemDetails problemDetails)
    {
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest,
        };
    }
}