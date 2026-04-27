using Distags.Core.Commands.LevelAddTag;
using Distags.Core.Commands.LevelRemoveTag;
using Distags.Core.Filtering.Levels;
using Distags.Core.Queries;
using Distags.Core.Queries.BrowseLevels;
using Distags.Core.Queries.LevelById;
using Distags.Core.Queries.LevelTags;
using LevelTagApi.ApiState;
using LevelTagApi.Dtos.Levels;
using LevelTagApi.Exceptions;
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
    public async Task<ActionResult<IEnumerable<LevelDto>>> BrowseLevels(
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
    [TypeFilter<MaintenanceFilter>(IsReusable = true)]
    public async Task<IActionResult> PutLevelTag(long levelId, string tagName,
        [FromServices] LevelAddTagCommandHandler levelAddTagCommandHandler)
    {
        try
        {
            LevelAddTagCommand command = new LevelAddTagCommand
            {
                LevelId = levelId,
                TagName = tagName,
            };
            await levelAddTagCommandHandler.HandleAsync(command);
            return Created();
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
    [TypeFilter<MaintenanceFilter>(IsReusable = true)]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tagName,
        [FromServices] LevelRemoveTagCommandHandler levelRemoveTagCommandHandler)
    {
        try
        {
            LevelRemoveTagCommand command = new LevelRemoveTagCommand
            {
                LevelId = levelId,
                TagName = tagName,
            };
            await levelRemoveTagCommandHandler.HandleAsync(command);
            return NoContent();
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