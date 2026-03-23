using DataClasses;
using DataStoring;
using LevelTagApi.DtoConverter;
using LevelTagApi.Dtos;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LevelsController : ControllerBase
{
    private readonly LevelTagDbContext _LevelTagDbContext;
    private readonly IDtoConverter<Level, GetLevelDto> _LevelDtoConverter;
    private readonly IQueryableSorterFactory<Level, LevelSortProperty> _LevelSorterFactory;

    public LevelsController(
        LevelTagDbContext levelTagDbContext,
        IDtoConverter<Level, GetLevelDto> levelDtoConverter,
        IQueryableSorterFactory<Level, LevelSortProperty> levelSorterFactory)
    {
        _LevelTagDbContext = levelTagDbContext;
        _LevelDtoConverter = levelDtoConverter;
        _LevelSorterFactory = levelSorterFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetLevelDto>>> GetLevels([FromQuery] GetLevelsParameters parameters)
    {
        IQueryable<Level> levels = _LevelTagDbContext.Levels;

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
            .Select(level => new GetLevelDto
            {
                Id = level.Id,
                Name = level.Name,
                Creator = level.Creator!.Name,
                FileName = level.FileName,
                FileUrl = level.FileUrl,
                IconUrl = level.IconUrl,
                CreatedOn = level.CreatedOn,
                UpdatedOn = level.UpdatedOn,
            })
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
        Level? level = await _LevelTagDbContext.Levels.FindAsync(levelId);
        if (level == null)
            return NotFound();

        GetLevelDto levelDto = _LevelDtoConverter.ToDto(level);
        return Ok(levelDto);
    }

    [HttpPut("batch")]
    public async Task<IActionResult> PutLevels([FromBody] List<PutLevelDto> levelDtos)
    {
        foreach (PutLevelDto levelDto in levelDtos)
        {
            Level? level = await _LevelTagDbContext.Levels.FindAsync(levelDto.Id);

            if (level == null)
            {
                level = new Level();
                level.Id = levelDto.Id;
                _LevelTagDbContext.Levels.Add(level);
            }

            level.Name = levelDto.Name;
            level.CreatorId = levelDto.CreatorId;
            level.FileName = levelDto.FileName;
            level.FileUrl = levelDto.FileUrl;
            level.IconUrl = levelDto.IconUrl;
            level.CreatedOn = levelDto.CreatedOn;
            level.UpdatedOn = levelDto.UpdatedOn;
        }

        await _LevelTagDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{levelId}")]
    public async Task<IActionResult> DeleteLevel(long levelId)
    {
        Level? level = await _LevelTagDbContext.Levels.FindAsync(levelId);
        if (level == null)
            return NotFound();

        _LevelTagDbContext.Levels.Remove(level);
        await _LevelTagDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{levelId}/tags")]
    public async Task<ActionResult<IEnumerable<string>>> GetLevelTags(long levelId)
    {
        Level? level = await _LevelTagDbContext.Levels.FindAsync(levelId);

        if (level == null)
            return NotFound();

        await _LevelTagDbContext.Entry(level).Collection(level => level.Tags).LoadAsync();

        return level.Tags.Select(tag => tag.Name).ToList();
    }

    [HttpPost("{levelId}/tags")]
    public async Task<IActionResult> PostLevelTag(long levelId, [FromBody] AssignTagRequest request)
    {
        Level? level = await _LevelTagDbContext.Levels.FindAsync(levelId);
        if (level == null)
            return NotFound($"Level {levelId} not found.");

        Tag tag = await GetOrCreateTag(request.Tag);

        if (await _LevelTagDbContext.LevelTags.FindAsync(levelId, tag.Id) != null)
            return NoContent();

        LevelTag levelTag = new LevelTag
        {
            LevelId = levelId,
            TagId = tag.Id,
        };

        _LevelTagDbContext.LevelTags.Add(levelTag);
        await _LevelTagDbContext.SaveChangesAsync();

        return Created(
            string.Empty,
            new
            {
                level_id = levelId,
                tag = tag.Name,
            });
    }

    [HttpDelete("{levelId}/tags/{tag}")]
    public async Task<IActionResult> DeleteLevelTag(long levelId, string tag)
    {
        Level? level = await _LevelTagDbContext.Levels.FindAsync(levelId);

        if (level == null)
            return NotFound();

        LevelTag? levelTag = await _LevelTagDbContext.LevelTags
            .FirstOrDefaultAsync(levelTag => levelTag.LevelId == levelId && levelTag.Tag.Name == tag);

        if (levelTag != null)
        {
            _LevelTagDbContext.LevelTags.Remove(levelTag);
            await _LevelTagDbContext.SaveChangesAsync();
        }

        return NoContent();
    }

    private async Task<Tag> GetOrCreateTag(string tagName)
    {
        Tag? tag = _LevelTagDbContext.Tags.FirstOrDefault(tag => tag.Name == tagName);
        if (tag != null)
            return tag;

        tag = new Tag { Name = tagName };
        _LevelTagDbContext.Tags.Add(tag);

        try
        {
            await _LevelTagDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            tag = _LevelTagDbContext.Tags.First(tag => tag.Name == tagName);
        }

        return tag;
    }
}