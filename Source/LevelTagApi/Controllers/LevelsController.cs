using DataClasses;
using DataStoring;
using LevelTagApi.ApiState;
using LevelTagApi.Dtos;
using LevelTagApi.Exceptions;
using LevelTagApi.Filtering;
using LevelTagApi.Management;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LevelsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public LevelsController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LevelDto>>> BrowseLevels(
        [FromQuery] BrowseLevelsParameters parameters,
        [FromServices] IQueryableFilterFactory<Level, LevelFilterParameters> levelFilterFactory,
        [FromServices] IQueryableSorterFactory<Level, LevelSortStrategy> levelSorterFactory)
    {
        BrowseLevelsResponse response = new BrowseLevelsResponse
        {
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
        };

        IQueryable<Level> levels = _DbContext.Set<Level>();

        LevelFilterParameters filterParams = GetFilterParameters(parameters);
        IQueryableFilter<Level> levelFilter = levelFilterFactory.GetQueryableFilter(filterParams);
        levels = levelFilter.Filter(levels);

        response.TotalLevels = await levels.CountAsync();
        if (response.TotalLevels == 0)
            return Ok(response);

        response.TotalPages = (int)Math.Ceiling((double)response.TotalLevels / parameters.PageSize);
        if (parameters.PageNumber > response.TotalPages)
            return Ok(response);

        IQueryableSorter<Level> levelSorter = levelSorterFactory.GetQueryableSorter(parameters.SortStrategy);
        levels = levelSorter.Sort(levels);

        if (parameters.PageNumber > 1)
            levels = levels.Skip((parameters.PageNumber - 1) * parameters.PageSize);

        if (response.TotalLevels > parameters.PageSize)
            levels = levels.Take(parameters.PageSize);

        List<LevelDto> levelDtos = await levels
            .Include(level => level.Creator)
            .Include(level => level.Tags)
            .Select(level => level.ToDto())
            .ToListAsync();

        response.Levels = levelDtos;
        return Ok(response);
    }

    [HttpGet("{levelId:long}")]
    [AllowAnonymous]
    public async Task<ActionResult<LevelDto>> GetLevel(long levelId)
    {
        Level? level = await _DbContext.Set<Level>()
            .Include(level => level.Creator)
            .Include(level => level.Tags)
            .FirstOrDefaultAsync(level => level.Id == levelId);

        if (level == null)
            return NotFound();

        LevelDto levelDto = level.ToDto();
        return Ok(levelDto);
    }

    [HttpGet("{levelId:long}/tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetLevelTags(long levelId)
    {
        Level? level = await _DbContext.Set<Level>()
            .Include(level => level.Tags)
            .FirstOrDefaultAsync(level => level.Id == levelId);

        if (level == null)
            return LevelNotFound(levelId);

        return level.Tags.Select(tag => tag.Name).ToList();
    }

    [HttpPut("{levelId:long}/tags/{tagName}")]
    [Authorize(Policy = Constants.Policies.Tagging)]
    [TypeFilter<MaintenanceFilter>(IsReusable = true)]
    public async Task<IActionResult> PutLevelTag(long levelId, string tagName,
        [FromServices] ILevelTagManager levelTagManager)
    {
        try
        {
            await levelTagManager.AddTagToLevelAsync(levelId, tagName);
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
        [FromServices] ILevelTagManager levelTagManager)
    {
        try
        {
            await levelTagManager.RemoveTagFromLevelAsync(levelId, tagName);
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

    private LevelFilterParameters GetFilterParameters(BrowseLevelsParameters parameters)
    {
        return new LevelFilterParameters
        {
            LevelFilter = parameters.LevelFilter,
            CreatorFilter = parameters.CreatorFilter,
            IncludeTags = parameters.IncludeTags,
            ExcludeTags = parameters.ExcludeTags,
        };
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