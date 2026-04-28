using Distags.Core.Commands;
using Distags.Core.Commands.CreateApp;
using Distags.Core.Commands.DeleteApp;
using Distags.Core.Exceptions;
using Distags.Core.Queries;
using Distags.Core.Queries.AppByName;
using Distags.Core.Queries.Apps;
using LevelTagApi.Dtos.Apps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppDto = Distags.Core.Dtos.AppDto;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AppsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApps(
        [FromServices] IQueryHandler<AppsQuery, List<AppDto>> appsQueryHandler)
    {
        List<AppDto> appDtos = await appsQueryHandler.HandleAsync(new AppsQuery());
        return Ok(appDtos);
    }

    [HttpGet("{appName}")]
    [Authorize(Roles = Constants.Roles.AppManager)]
    public async Task<ActionResult<List<AppDto>>> GetApp(string appName,
        [FromServices] IQueryHandler<AppByNameQuery, AppDto> appByNameQueryHandler)
    {
        try
        {
            AppByNameQuery query = new AppByNameQuery { AppName = appName };
            AppDto appDto = await appByNameQueryHandler.HandleAsync(query);
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
        [FromServices] ICommandHandler<CreateAppCommand> createAppCommandHandler)
    {
        try
        {
            CreateAppCommand command = new CreateAppCommand { AppName = createAppDto.Name };
            await createAppCommandHandler.HandleAsync(command);
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
        [FromServices] ICommandHandler<DeleteAppCommand> deleteAppCommandHandler)
    {
        try
        {
            DeleteAppCommand command = new DeleteAppCommand { AppName = appName };
            await deleteAppCommandHandler.HandleAsync(command);
            return NoContent();
        }
        catch (UnknownAppNameException ex)
        {
            return NotFound();
        }
    }
}