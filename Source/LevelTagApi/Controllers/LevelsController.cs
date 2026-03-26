using DataClasses;
using DataStoring;
using LevelTagApi.ApiState;
using LevelTagApi.Dtos;
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
    private readonly IQueryableFilterFactory<Level, LevelFilterParameters> _LevelFilterFactory;
    private readonly IQueryableSorterFactory<Level, LevelSortStrategy> _LevelSorterFactory;
    private readonly ILevelTagManager _LevelTagManager;

    public LevelsController(
        LevelTagDbContext dbContext,
        IQueryableFilterFactory<Level, LevelFilterParameters> levelFilterFactory,
        IQueryableSorterFactory<Level, LevelSortStrategy> levelSorterFactory,
        ILevelTagManager levelTagManager)
    {
        _DbContext = dbContext;
        _LevelFilterFactory = levelFilterFactory;
        _LevelSorterFactory = levelSorterFactory;
        _LevelTagManager = levelTagManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<LevelDto>>> BrowseLevels([FromQuery] BrowseLevelsParameters parameters)
    {
        BrowseLevelsResponse response = new BrowseLevelsResponse
        {
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
        };

        IQueryable<Level> levels = _DbContext.Set<Level>();

        LevelFilterParameters filterParams = GetFilterParameters(parameters);
        IQueryableFilter<Level> levelFilter = _LevelFilterFactory.GetQueryableFilter(filterParams);
        levels = levelFilter.Filter(levels);

        response.TotalLevels = await levels.CountAsync();
        if (response.TotalLevels == 0)
            return Ok(response);

        response.TotalPages = (int)Math.Ceiling((double)response.TotalLevels / parameters.PageSize);
        if (parameters.PageNumber > response.TotalPages)
            return Ok(response);

        IQueryableSorter<Level> levelSorter = _LevelSorterFactory.GetQueryableSorter(parameters.SortStrategy);
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

    [HttpPost("{levelId:long}/tags/{tagName}")]
    [Authorize(Policy = Constants.Policies.Tagging)]
    [TypeFilter<MaintenanceFilter>(IsReusable = true)]
    public async Task<IActionResult> PostLevelTag(long levelId, string tagName)
    {
        Level? level = await _DbContext.Set<Level>().FirstOrDefaultAsync(level => level.Id == levelId);
        if (level == null)
            return LevelNotFound(levelId);

        Tag? tag = await _DbContext.Set<Tag>()
            .Include(tag => tag.Group)
            .FirstOrDefaultAsync(tag => tag.Name == tagName);
        if (tag == null)
            return BadRequest($"Tag '{tagName}' is not supported.");

        if (await _DbContext.Set<LevelTag>().FindAsync(levelId, tag.Id) != null)
            return NoContent();

        await _LevelTagManager.AddTagToLevelAsync(level, tag);

        return Created();
    }

    [HttpDelete("{levelId:long}/tags/{tagName}")]
    [Authorize(Policy = Constants.Policies.Tagging)]
    [TypeFilter<MaintenanceFilter>(IsReusable = true)]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tagName)
    {
        Level? level = await _DbContext.Set<Level>().FindAsync(levelId);
        if (level == null)
            return LevelNotFound(levelId);

        LevelTag? levelTag = await _DbContext.Set<LevelTag>()
            .FirstOrDefaultAsync(levelTag => levelTag.LevelId == levelId && levelTag.Tag.Name == tagName);

        if (levelTag != null)
            await _LevelTagManager.RemoveTagFromLevelAsync(levelTag);

        return NoContent();
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
        return NotFound($"Could not find level with ID {levelId}.");
    }
}