using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public TagsController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<TagGroupDto>>> GetTagTreeAsync()
    {
        List<TagGroupDto> tagGroupTree = await _DbContext.Set<TagGroup>()
            .Include(tagGroup => tagGroup.Tags)
            .OrderBy(tagGroup => tagGroup.Order)
            .Select(tagGroup => tagGroup.ToDto())
            .ToListAsync();

        return Ok(tagGroupTree);
    }
}