using Distags.Api.Contracts.Levels;
using Distags.Api.Security;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Exceptions;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries.BrowseLevels;
using Distags.Core.Queries.LevelById;
using Distags.Core.Queries.LevelTags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LevelSortStrategy = Distags.Core.Sorting.Levels.LevelSortStrategy;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LevelsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<LevelsPage>> BrowseLevels(
        [FromQuery] BrowseLevelsParameters parameters,
        [FromServices] IRequestDispatcher dispatcher)
    {
        BrowseLevelsQuery query = new BrowseLevelsQuery
        {
            FilterParameters = new LevelFilterParameters
            {
                LevelName = parameters.LevelName,
                CreatorName = parameters.CreatorName,
                IncludeTags = GetNormalizedTagNames(parameters.IncludeTags),
                ExcludeTags = GetNormalizedTagNames(parameters.ExcludeTags),
            },
            SortStrategy = (LevelSortStrategy)parameters.SortStrategy,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
        };

        LevelsPage levelsPage = await dispatcher.DispatchAsync(query);
        return Ok(levelsPage);
    }

    private List<string> GetNormalizedTagNames(string? commaSeparatedTags)
    {
        if (string.IsNullOrEmpty(commaSeparatedTags))
            return [];

        string[] tagNames = commaSeparatedTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<string> normalizedTagNames = tagNames.Select(tag => tag.ToLowerInvariant()).ToList();
        return normalizedTagNames;
    }

    [HttpGet("{levelId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<LevelDto>> GetLevel(long levelId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            LevelDto levelDto = await requestDispatcher.DispatchAsync(new LevelByIdQuery { LevelId = levelId });
            return Ok(levelDto);
        }
        catch (UnknownLevelException)
        {
            return ProblemDetailsResult.LevelNotFound(levelId);
        }
    }

    [HttpGet("{levelId:long}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetLevelTags(long levelId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            List<string> tags = await requestDispatcher.DispatchAsync(new LevelTagsQuery { LevelId = levelId });
            return Ok(tags);
        }
        catch (UnknownLevelException)
        {
            return ProblemDetailsResult.LevelNotFound(levelId);
        }
    }

    [HttpPut("{levelId:long}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Tagger)]
    public async Task<IActionResult> PutLevelTag(long levelId, string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeLevelTagCommand command = new ChangeLevelTagCommand
            {
                LevelId = levelId,
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
            return ProblemDetailsResult.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return ProblemDetailsResult.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return ProblemDetailsResult.LevelNotFound(levelId);
        }
        catch (UnknownTagException)
        {
            return ProblemDetailsResult.TagNotFound(tagName);
        }
        catch (LevelTagExistsException)
        {
            return ProblemDetailsResult.LevelTagConflict(levelId, tagName);
        }
    }

    [HttpDelete("{levelId:long}/tags/{tagName}")]
    [Authorize(Roles = Constants.Roles.Tagger)]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tagName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            ChangeLevelTagCommand command = new ChangeLevelTagCommand
            {
                LevelId = levelId,
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
            return ProblemDetailsResult.MaintenanceActive();
        }
        catch (CannotChangeAutoTagException)
        {
            return ProblemDetailsResult.ForbiddenChangeAutoTag(tagName);
        }
        catch (UnknownLevelException)
        {
            return ProblemDetailsResult.LevelNotFound(levelId);
        }
        catch (UnknownTagException)
        {
            return ProblemDetailsResult.TagNotFound(tagName);
        }
        catch (LevelTagNotFoundException)
        {
            return ProblemDetailsResult.LevelTagNotFound(levelId, tagName);
        }
    }
}