using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Hubs;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LevelsController : ControllerBase
{
    private readonly LevelTagDbContext _LevelTagDbContext;
    private readonly IQueryableSorterFactory<Level, LevelSortProperty> _LevelSorterFactory;

    public LevelsController(
        LevelTagDbContext levelTagDbContext,
        IQueryableSorterFactory<Level, LevelSortProperty> levelSorterFactory)
    {
        _LevelTagDbContext = levelTagDbContext;
        _LevelSorterFactory = levelSorterFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetLevelDto>>> GetLevels([FromQuery] GetLevelsParameters parameters)
    {
        IQueryable<Level> levels = _LevelTagDbContext.Set<Level>();

        if (!string.IsNullOrEmpty(parameters.IncludeTags))
        {
            IEnumerable<string> includeTagNames = GetNormalizedTagNames(parameters.IncludeTags);
            levels = levels.Where(level => includeTagNames.All(tagName => level.Tags.Any(tag => tag.Name == tagName)));
        }

        if (!string.IsNullOrEmpty(parameters.ExcludeTags))
        {
            IEnumerable<string> excludeTagNames = GetNormalizedTagNames(parameters.ExcludeTags);
            levels = levels.Where(level => !excludeTagNames.Any(tagName => level.Tags.Any(tag => tag.Name == tagName)));
        }

        int totalLevelCount = await levels.CountAsync();

        IQueryableSorter<Level> levelSorter = _LevelSorterFactory.GetQueryableSorter(parameters.SortBy);
        levels = levelSorter.Sort(levels, parameters.SortOrder);

        if (totalLevelCount > parameters.PageSize)
        {
            if (parameters.PageNumber > 1)
                levels = levels.Skip((parameters.PageNumber - 1) * parameters.PageSize);

            levels = levels.Take(parameters.PageSize);
        }

        List<GetLevelDto> levelDtos = await levels
            .Include(level => level.Creator)
            .Include(level => level.Tags)
            .Select(level => level.ToDto())
            .ToListAsync();

        GetLevelsResponse response = new GetLevelsResponse
        {
            TotalLevels = totalLevelCount,
            Levels = levelDtos,
        };

        return Ok(response);
    }

    private IEnumerable<string> GetNormalizedTagNames(string commaSeparatedTags)
    {
        string[] tagNames = commaSeparatedTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IEnumerable<string> normalizedTagNames = tagNames.Select(tag => tag.ToLowerInvariant());
        return normalizedTagNames;
    }

    [HttpGet("{levelId}")]
    public async Task<ActionResult<GetLevelDto>> GetLevel(long levelId)
    {
        Level? level = await _LevelTagDbContext.Set<Level>()
            .Include(level => level.Creator)
            .Include(level => level.Tags)
            .FirstOrDefaultAsync(level => level.Id == levelId);

        if (level == null)
            return NotFound();

        GetLevelDto levelDto = level.ToDto();
        return Ok(levelDto);
    }

    [HttpGet("{levelId}/tags")]
    public async Task<ActionResult<IEnumerable<string>>> GetLevelTags(long levelId)
    {
        Level? level = await _LevelTagDbContext.Set<Level>()
            .Include(level => level.Tags)
            .FirstOrDefaultAsync(level => level.Id == levelId);

        if (level == null)
            return LevelNotFound(levelId);


        return level.Tags.Select(tag => tag.Name).ToList();
    }

    [HttpPost("{levelId}/tags/{tagName}")]
    public async Task<IActionResult> PostLevelTag(long levelId, string tagName, [FromServices] IHubContext<LevelHub> levelHub)
    {
        Level? level = await _LevelTagDbContext.Set<Level>().FirstOrDefaultAsync(level => level.Id == levelId);
        if (level == null)
            return LevelNotFound(levelId);

        Tag? tag = await _LevelTagDbContext.Set<Tag>()
            .Include(tag => tag.Group)
            .FirstOrDefaultAsync(tag => tag.Name == tagName);
        if (tag == null)
            return BadRequest($"Tag '{tagName}' is not supported.");

        if (await _LevelTagDbContext.Set<LevelTag>().FindAsync(levelId, tag.Id) != null)
            return NoContent();

        List<LevelTag>? conflictingLevelTags = null;
        if (tag.Exclusivity != null)
        {
            conflictingLevelTags = await _LevelTagDbContext.Set<LevelTag>()
                .Include(levelTag => levelTag.Tag)
                .Where(levelTag => levelTag.Level == level && levelTag.Tag.Exclusivity == tag.Exclusivity)
                .ToListAsync();

            foreach (LevelTag conflictingLevelTag in conflictingLevelTags)
            {
                _LevelTagDbContext.Remove(conflictingLevelTag);
            }
        }

        LevelTag levelTag = new LevelTag
        {
            Level = level,
            Tag = tag,
        };

        _LevelTagDbContext.Add(levelTag);
        await _LevelTagDbContext.SaveChangesAsync();

        if (conflictingLevelTags?.Any() == true)
        {
            foreach (LevelTag conflictingLevelTag in conflictingLevelTags)
            {
                await levelHub.Clients.Group(levelId.ToString()).SendAsync("TagRemoved", conflictingLevelTag.Tag.Name);
            }
        }
        await levelHub.Clients.Group(levelId.ToString()).SendAsync("TagAdded", tagName);

        return Created();
    }

    [HttpDelete("{levelId}/tags/{tagName}")]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tagName, [FromServices] IHubContext<LevelHub> levelHub)
    {
        Level? level = await _LevelTagDbContext.Set<Level>().FindAsync(levelId);

        if (level == null)
            return LevelNotFound(levelId);

        LevelTag? levelTag = await _LevelTagDbContext.Set<LevelTag>()
            .FirstOrDefaultAsync(levelTag => levelTag.LevelId == levelId && levelTag.Tag.Name == tagName);

        if (levelTag != null)
        {
            _LevelTagDbContext.Remove(levelTag);
            await _LevelTagDbContext.SaveChangesAsync();

            await levelHub.Clients.Group(levelId.ToString()).SendAsync("TagRemoved", tagName);
        }

        return NoContent();
    }

    private NotFoundObjectResult LevelNotFound(long levelId)
    {
        return NotFound($"Could not find level with ID {levelId}.");
    }
}