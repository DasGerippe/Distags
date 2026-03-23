using DataStoring;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public StatsController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }
}