using Microsoft.AspNetCore.Mvc;

namespace Distags.Api;

public static class DistagsProblemResults
{
    public static ProblemResult ForbiddenChangeAutoTag(string autoTagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Cannot Change Auto-Applied Tag",
            Detail = $"You cannot change the tag '{autoTagName}' because it is applied automatically.",
        });
    }

    public static ProblemResult BadRequestRemoveFallbackTag(string fallbackTagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Cannot Remove Fallback Tag",
            Detail = $"You cannot remove the fallback tag '{fallbackTagName}' because it is the last one of it's group.",
        });
    }

    public static ProblemResult ForbiddenBanSelf()
    {
        return ForbiddenBan("You cannot ban yourself.");
    }

    public static ProblemResult ForbiddenBanOtherUser()
    {
        return ForbiddenBan("You cannot ban the user of that Steam ID.");
    }

    private static ProblemResult ForbiddenBan(string detail)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Cannot Ban Steam ID",
            Detail = detail,
        });
    }

    public static ProblemResult MaintenanceActive()
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Maintenance Active",
            Detail = "This operation is temporarily unavailable due to maintenance.",
        });
    }

    public static ProblemResult OfficialLevelNotFound(string levelSlug)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Official Level Not Found",
            Detail = $"Could not find official level {levelSlug}.",
        });
    }

    public static ProblemResult WorkshopLevelNotFound(long levelWorkshopId)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Workshop Level Not Found",
            Detail = $"Could not find workshop level {levelWorkshopId}.",
        });
    }

    public static ProblemResult OfficialLevelTagNotFound(string levelSlug, string tagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Level Tag Not Found",
            Detail = $"Could not find tag '{tagName}' on official level {levelSlug}.",
        });
    }

    public static ProblemResult WorkshopLevelTagNotFound(long levelWorkshopId, string tagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Level Tag Not Found",
            Detail = $"Could not find tag '{tagName}' on workshop level {levelWorkshopId}.",
        });
    }

    public static ProblemResult OfficialLevelTagConflict(string levelSlug, string tagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Level Tag Already Exists",
            Detail = $"Could not add tag '{tagName}' to official level {levelSlug} because it is already present.",
        });
    }

    public static ProblemResult WorkshopLevelTagConflict(long levelWorkshopId, string tagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Level Tag Already Exists",
            Detail = $"Could not add tag '{tagName}' to workshop level {levelWorkshopId} because it is already present.",
        });
    }

    public static ProblemResult TagNotFound(string tagName)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Tag Not Found",
            Detail = $"Could not find tag with name '{tagName}'.",
        });
    }

    public static ProblemResult UserNotFound(long userSteamId)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "User Not Found",
            Detail = $"Could not find user with Steam ID {userSteamId}.",
        });
    }

    public static ProblemResult UserBanNotFound(long steamId)
    {
        return new ProblemResult(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "User Ban Not Found",
            Detail = $"The Steam ID {steamId} is not banned.",
        });
    }

    public static ProblemResult UserBanConflict(long steamId)
    {
        return new ProblemResult(new ProblemDetails
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
