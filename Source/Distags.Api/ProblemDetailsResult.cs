using Microsoft.AspNetCore.Mvc;

namespace Distags.Api;

public static class ProblemDetailsResult
{
    public static ObjectResult ForbiddenChangeAutoTag(string autoTagName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Cannot Change Auto-Applied Tag",
            Detail = $"You cannot change the tag '{autoTagName}' because it is applied automatically.",
        });
    }

    public static ObjectResult MaintenanceActive()
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Maintenance Active",
            Detail = "This operation is temporarily unavailable due to maintenance.",
        });
    }

    public static ObjectResult AppNotFound(string appName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "App Not Found",
            Detail = $"Could not find app with name '{appName}'.",
        });
    }

    public static ObjectResult AppConflict(string appName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "App Already Exists",
            Detail = $"An app with the name '{appName}' already exists.",
        });
    }

    public static ObjectResult LevelNotFound(long levelId)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Level Not Found",
            Detail = $"Could not find level with ID {levelId}.",
        });
    }

    public static ObjectResult LevelTagNotFound(long levelId, string tagName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Level Tag Not Found",
            Detail = $"Could not find tag '{tagName}' on level {levelId}.",
        });
    }

    public static ObjectResult LevelTagConflict(long levelId, string tagName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Level Tag Already Exists",
            Detail = $"Could not add tag '{tagName}' to level {levelId} because it is already present.",
        });
    }

    public static ObjectResult TagNotFound(string tagName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Tag Not Found",
            Detail = $"Could not find tag with name '{tagName}'.",
        });
    }

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