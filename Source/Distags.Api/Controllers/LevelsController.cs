using Distags.Api.Contracts.Levels;
using Distags.Api.Mapping;
using Distags.Api.Security;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Exceptions;
using Distags.Core.Queries.BrowseLevels;
using Distags.Core.Queries.OfficialLevelBySlug;
using Distags.Core.Queries.WorkshopLevelById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LevelsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<LevelsPageResponse>> BrowseLevels(
        [FromQuery] BrowseLevelsRequest request,
        [FromServices] IRequestDispatcher dispatcher)
    {
        BrowseLevelsQuery query = BrowseLevelsMapper.Map(request);
        LevelsPage levelsPage = await dispatcher.DispatchAsync(query);
        LevelsPageResponse levelsPageResponse = LevelsPageMapper.Map(levelsPage);
        return Ok(levelsPageResponse);
    }

    [HttpGet("official/{levelSlug}")]
    [AllowAnonymous]
    public async Task<ActionResult<OfficialLevelResponse>> GetOfficialLevel(
        string levelSlug,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            OfficialLevelBySlugQuery query = new OfficialLevelBySlugQuery { Slug = levelSlug };
            OfficialLevelDto levelDto = await requestDispatcher.DispatchAsync(query);
            OfficialLevelResponse levelResponse = LevelDetailMapper.Map(levelDto);
            return Ok(levelResponse);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.OfficialLevelNotFound(levelSlug);
        }
    }

    [HttpGet("workshop/{levelWorkshopId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<WorkshopLevelResponse>> GetWorkshopLevel(
        long levelWorkshopId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            WorkshopLevelByIdQuery query = new WorkshopLevelByIdQuery { WorkshopId = levelWorkshopId };
            WorkshopLevelDto levelDto = await requestDispatcher.DispatchAsync(query);
            WorkshopLevelResponse levelResponse = LevelDetailMapper.Map(levelDto);
            return Ok(levelResponse);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.WorkshopLevelNotFound(levelWorkshopId);
        }
    }

    [HttpGet("official/{levelSlug}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetOfficialLevelTags(
        string levelSlug,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            OfficialLevelBySlugQuery query = new OfficialLevelBySlugQuery { Slug = levelSlug };
            OfficialLevelDto level = await requestDispatcher.DispatchAsync(query);
            List<string> tags = level.Tags;
            return Ok(tags);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.OfficialLevelNotFound(levelSlug);
        }
    }

    [HttpGet("workshop/{levelWorkshopId:long}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetWorkshopLevelTags(
        long levelWorkshopId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            WorkshopLevelByIdQuery query = new WorkshopLevelByIdQuery { WorkshopId = levelWorkshopId };
            WorkshopLevelDto level = await requestDispatcher.DispatchAsync(query);
            List<string> tags = level.Tags;
            return Ok(tags);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.WorkshopLevelNotFound(levelWorkshopId);
        }
    }

    [HttpPut("official/{levelSlug}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Contributor)]
    public async Task<IActionResult> PutOfficialLevelTag(
        string levelSlug,
        string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeOfficialLevelTagCommand command = new ChangeOfficialLevelTagCommand
            {
                LevelSlug = levelSlug,
                TagName = tagName,
                UserSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId),
                AppName = User.FindFirstValue(Constants.Claims.AppName),
                Action = TagChangeAction.Add,
            };
            await requestDispatcher.DispatchAsync(command);
            return Created();
        }
        catch (MaintenanceActiveException)
        {
            return DistagsProblemResults.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return DistagsProblemResults.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.OfficialLevelNotFound(levelSlug);
        }
        catch (UnknownTagException)
        {
            return DistagsProblemResults.TagNotFound(tagName);
        }
        catch (LevelTagExistsException)
        {
            return DistagsProblemResults.OfficialLevelTagConflict(levelSlug, tagName);
        }
    }

    [HttpPut("workshop/{levelWorkshopId:long}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Contributor)]
    public async Task<IActionResult> PutWorkshopLevelTag(
        long levelWorkshopId,
        string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeWorkshopLevelTagCommand command = new ChangeWorkshopLevelTagCommand
            {
                LevelWorkshopId = levelWorkshopId,
                TagName = tagName,
                UserSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId),
                AppName = User.FindFirstValue(Constants.Claims.AppName),
                Action = TagChangeAction.Add,
            };
            await requestDispatcher.DispatchAsync(command);
            return Created();
        }
        catch (MaintenanceActiveException)
        {
            return DistagsProblemResults.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return DistagsProblemResults.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.WorkshopLevelNotFound(levelWorkshopId);
        }
        catch (UnknownTagException)
        {
            return DistagsProblemResults.TagNotFound(tagName);
        }
        catch (LevelTagExistsException)
        {
            return DistagsProblemResults.WorkshopLevelTagConflict(levelWorkshopId, tagName);
        }
    }

    [HttpDelete("official/{levelSlug}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Contributor)]
    public async Task<IActionResult> DeleteOfficialLevelTag(
        string levelSlug,
        string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeOfficialLevelTagCommand command = new ChangeOfficialLevelTagCommand
            {
                LevelSlug = levelSlug,
                TagName = tagName,
                UserSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId),
                AppName = User.FindFirstValue(Constants.Claims.AppName),
                Action = TagChangeAction.Remove,
            };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (MaintenanceActiveException)
        {
            return DistagsProblemResults.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return DistagsProblemResults.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.OfficialLevelNotFound(levelSlug);
        }
        catch (UnknownTagException)
        {
            return DistagsProblemResults.TagNotFound(tagName);
        }
        catch (LevelTagNotFoundException)
        {
            return DistagsProblemResults.OfficialLevelTagNotFound(levelSlug, tagName);
        }
        catch (CannotRemoveFallbackTagException)
        {
            return DistagsProblemResults.BadRequestRemoveFallbackTag(tagName);
        }
    }

    [HttpDelete("workshop/{levelWorkshopId:long}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Contributor)]
    public async Task<IActionResult> DeleteWorkshopLevelTag(
        long levelWorkshopId,
        string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeWorkshopLevelTagCommand command = new ChangeWorkshopLevelTagCommand
            {
                LevelWorkshopId = levelWorkshopId,
                TagName = tagName,
                UserSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId),
                AppName = User.FindFirstValue(Constants.Claims.AppName),
                Action = TagChangeAction.Remove,
            };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (MaintenanceActiveException)
        {
            return DistagsProblemResults.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return DistagsProblemResults.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return DistagsProblemResults.WorkshopLevelNotFound(levelWorkshopId);
        }
        catch (UnknownTagException)
        {
            return DistagsProblemResults.TagNotFound(tagName);
        }
        catch (LevelTagNotFoundException)
        {
            return DistagsProblemResults.WorkshopLevelTagNotFound(levelWorkshopId, tagName);
        }
        catch (CannotRemoveFallbackTagException)
        {
            return DistagsProblemResults.BadRequestRemoveFallbackTag(tagName);
        }
    }
}
