using Distags.Api.Contracts.Apps;
using Distags.Core;
using Distags.Core.Commands.CreateApp;
using Distags.Core.Commands.DeleteApp;
using Distags.Core.Exceptions;
using Distags.Core.Queries.AppByName;
using Distags.Core.Queries.Apps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDto = Distags.Contracts.AppDto;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApps(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<AppDto> appDtos = await requestDispatcher.DispatchAsync(new AppsQuery());
        return Ok(appDtos);
    }

    [HttpGet("{appName}")]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApp(string appName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            AppByNameQuery query = new AppByNameQuery { AppName = appName };
            AppDto appDto = await requestDispatcher.DispatchAsync(query);
            return Ok(appDto);
        }
        catch (UnknownAppNameException ex)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<AppDto>> CreateApp([FromBody] CreateAppDto createAppDto,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            CreateAppCommand command = new CreateAppCommand { AppName = createAppDto.Name };
            await requestDispatcher.DispatchAsync(command);
            return CreatedAtAction(nameof(GetApp), new { appName = createAppDto.Name }, null);
        }
        catch (AppNameTakenException ex)
        {
            return Conflict();
        }
    }

    [HttpDelete("{appName}")]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<IActionResult> DeleteApiKey(string appName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            DeleteAppCommand command = new DeleteAppCommand { AppName = appName };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (UnknownAppNameException ex)
        {
            return NotFound();
        }
    }
}