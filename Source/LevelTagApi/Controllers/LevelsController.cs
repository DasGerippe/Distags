using Distags.Core.Commands;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Dtos;
using Distags.Core.Exceptions;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries;
using Distags.Core.Queries.BrowseLevels;
using Distags.Core.Queries.LevelById;
using Distags.Core.Queries.LevelTags;
using LevelTagApi.Dtos.Levels;
using LevelTagApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LevelDto = Distags.Core.Dtos.LevelDto;
using LevelSortStrategy = Distags.Core.Sorting.Levels.LevelSortStrategy;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LevelsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<LevelsPage>> BrowseLevels(
        [FromQuery] BrowseLevelsParameters parameters,
        [FromServices] IQueryHandler<BrowseLevelsQuery, LevelsPage> browseLevelsQueryHandler)
    {
        BrowseLevelsQuery query = new BrowseLevelsQuery
        {
            FilterParameters = new LevelFilterParameters
            {
                LevelFilter = parameters.LevelFilter,
                CreatorFilter = parameters.CreatorFilter,
                IncludeTags = GetNormalizedTagNames(parameters.IncludeTags),
                ExcludeTags = GetNormalizedTagNames(parameters.ExcludeTags),
            },
            SortStrategy = (LevelSortStrategy)parameters.SortStrategy,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
        };

        LevelsPage levelsPage = await browseLevelsQueryHandler.HandleAsync(query);
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
        [FromServices] IQueryHandler<LevelByIdQuery, LevelDto> levelByIdQueryHandler)
    {
        try
        {
            LevelDto levelDto = await levelByIdQueryHandler.HandleAsync(new LevelByIdQuery { LevelId = levelId });
            return Ok(levelDto);
        }
        catch (UnknownLevelException ex)
        {
            return LevelNotFound(ex.LevelId);
        }
    }

    [HttpGet("{levelId:long}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetLevelTags(long levelId,
        [FromServices] IQueryHandler<LevelTagsQuery, List<string>> levelTagsQueryHandler)
    {
        try
        {
            List<string> tags = await levelTagsQueryHandler.HandleAsync(new LevelTagsQuery { LevelId = levelId });
            return Ok(tags);
        }
        catch (UnknownLevelException ex)
        {
            return LevelNotFound(ex.LevelId);
        }
    }

    [HttpPut("{levelId:long}/tags/{tagName}")]
    [Authorize(Policy = Constants.Policies.Tagging)]
    public async Task<IActionResult> PutLevelTag(long levelId, string tagName,
        [FromServices] ICommandHandler<ChangeLevelTagCommand> changeLevelTagCommandHandler)
    {
        try
        {
            ChangeLevelTagCommand command = new ChangeLevelTagCommand
            {
                LevelId = levelId,
                TagName = tagName,
                UserSteamId = User.GetSteamId(),
                AppName = User.GetAppName(),
                ChangeAction = ChangeActionDto.AddTag,
            };
            await changeLevelTagCommandHandler.HandleAsync(command);
            return Created();
        }
        catch (MaintenanceActiveException)
        {
            return MaintenanceResult();
        }
        catch (UnknownLevelException ex)
        {
            return LevelNotFound(ex.LevelId);
        }
        catch (UnknownTagException ex)
        {
            return TagNotSupported(ex.TagName);
        }
        catch (LevelTagExistsException)
        {
            return NoContent();
        }
    }

    [HttpDelete("{levelId:long}/tags/{tagName}")]
    [Authorize(Policy = Constants.Policies.Tagging)]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tagName,
        [FromServices] ICommandHandler<ChangeLevelTagCommand> changeLevelTagCommandHandler)
    {
        try
        {
            ChangeLevelTagCommand command = new ChangeLevelTagCommand
            {
                LevelId = levelId,
                TagName = tagName,
                UserSteamId = User.GetSteamId(),
                AppName = User.GetAppName(),
                ChangeAction = ChangeActionDto.RemoveTag,
            };
            await changeLevelTagCommandHandler.HandleAsync(command);
            return NoContent();
        }
        catch (MaintenanceActiveException)
        {
            return MaintenanceResult();
        }
        catch (UnknownLevelException ex)
        {
            return LevelNotFound(ex.LevelId);
        }
        catch (UnknownTagException ex)
        {
            return TagNotSupported(ex.TagName);
        }
        catch (LevelTagNotFoundException)
        {
            return NotFound();
        }
    }

    private IActionResult MaintenanceResult()
    {
        return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
        {
            Title = "Maintenance Active",
            Detail = "This operation is temporarily unavailable due to maintenance.",
        });
    }

    private NotFoundObjectResult LevelNotFound(long levelId)
    {
        return NotFound(new ProblemDetails
        {
            Title = "Unknown level",
            Detail = $"Could not find level with ID {levelId}.",
        });
    }

    private BadRequestObjectResult TagNotSupported(string tagName)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Unsupported tag",
            Detail = $"The tag '{tagName}' is not supported.",
        });
    }
}