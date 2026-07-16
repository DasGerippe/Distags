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

    public static ObjectResult BadRequestRemoveFallbackTag(string fallbackTagName)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Cannot Remove Fallback Tag",
            Detail = $"You cannot remove the fallback tag '{fallbackTagName}' because it is the last one of it's group.",
        });
    }

    public static ObjectResult ForbiddenBanSelf()
    {
        return ForbiddenBan("You cannot ban yourself.");
    }

    public static ObjectResult ForbiddenBanOtherUser()
    {
        return ForbiddenBan("You cannot ban the user of that Steam ID.");
    }

    private static ObjectResult ForbiddenBan(string detail)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Cannot Ban Steam ID",
            Detail = detail,
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

    public static ObjectResult UserBanNotFound(long steamId)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "User Ban Not Found",
            Detail = $"The Steam ID {steamId} is not banned.",
        });
    }

    public static ObjectResult UserBanConflict(long steamId)
    {
        return ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Steam ID Already Banned",
            Detail = $"A ban for the Steam ID {steamId} already exists.",
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