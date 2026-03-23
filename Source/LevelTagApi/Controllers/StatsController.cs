using DataStoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly LevelTagDbContext _LevelTagDbContext;

    public StatsController(LevelTagDbContext levelTagDbContext)
    {
        _LevelTagDbContext = levelTagDbContext;
    }

    [HttpGet("last-level-sync")]
    public async Task<ActionResult<DateTime?>> GetLastLevelSyncDate()
    {
        if (!await _LevelTagDbContext.Levels.AnyAsync())
            return Ok(null);

        DateTime newestCreatedDate = await _LevelTagDbContext.Levels.MaxAsync(level => level.CreatedOn);
        DateTime newestUpdatedDate = await _LevelTagDbContext.Levels.MaxAsync(level => level.UpdatedOn);
        DateTime lastSyncDate = newestCreatedDate > newestUpdatedDate ? newestCreatedDate : newestUpdatedDate;

        return Ok(lastSyncDate);
    }
}