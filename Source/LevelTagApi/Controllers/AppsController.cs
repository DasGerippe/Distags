using DataClasses;
using DataStoring;
using LevelTagApi.Dtos.Apps;
using LevelTagApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AppsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly ITokenGenerator _TokenGenerator;

    public AppsController(LevelTagDbContext dbContext, ITokenGenerator tokenGenerator)
    {
        _DbContext = dbContext;
        _TokenGenerator = tokenGenerator;
    }

    [HttpGet]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApps()
    {
        List<AppDto> apps = await _DbContext.Set<App>()
            .Select(app => app.ToDto())
            .ToListAsync();

        return Ok(apps);
    }

    [HttpGet("{appName}")]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApp(string appName)
    {
        App? app = await _DbContext.Set<App>().FirstOrDefaultAsync(app => app.Name == appName);

        return app == null
            ? NotFound()
            : Ok(app.ToDto());
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<AppDto>> CreateApp([FromBody] CreateAppDto createAppDto)
    {
        string appName = createAppDto.Name;
        App? app = await _DbContext.Set<App>().FirstOrDefaultAsync(app => app.Name == appName);
        if (app != null)
            return Conflict($"{nameof(App)} with name {appName} already exists.");

        app = new App
        {
            Name = appName,
            ApiKey = _TokenGenerator.GenerateToken(32),
        };

        _DbContext.Add(app);
        await _DbContext.SaveChangesAsync();

        AppDto appDto = app.ToDto();
        return CreatedAtAction(nameof(GetApp), new { appName = appName }, appDto);
    }

    [HttpDelete("{appName}")]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<IActionResult> DeleteApiKey(string appName)
    {
        App? apiKey = await _DbContext.Set<App>().FirstOrDefaultAsync(app => app.Name == appName);
        if (apiKey == null)
            return NotFound();

        _DbContext.Remove(apiKey);
        await _DbContext.SaveChangesAsync();

        return NoContent();
    }
}