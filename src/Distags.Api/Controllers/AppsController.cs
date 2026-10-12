using Distags.Api.Contracts.Apps;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.CreateApp;
using Distags.Core.Commands.DeleteApp;
using Distags.Core.Exceptions;
using Distags.Core.Queries.AppByName;
using Distags.Core.Queries.Apps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AppsController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<ActionResult<List<AppResponse>>> GetApps(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<AppDto> appDtos = await requestDispatcher.DispatchAsync(new AppsQuery());
        List<AppResponse> appResponses = appDtos
            .Select(AppMapper.Map)
            .ToList();
        return Ok(appResponses);
    }

    [HttpGet("{appName}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<ActionResult<AppResponse>> GetApp(
        string appName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            AppByNameQuery query = new AppByNameQuery { AppName = appName };
            AppDto appDto = await requestDispatcher.DispatchAsync(query);
            AppResponse appResponse = AppMapper.Map(appDto);
            return Ok(appResponse);
        }
        catch (UnknownAppNameException)
        {
            return DistagsProblemResults.AppNotFound(appName);
        }
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> CreateApp(
        [FromBody] CreateAppRequest createAppDto,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            CreateAppCommand command = new CreateAppCommand { AppName = createAppDto.Name };
            await requestDispatcher.DispatchAsync(command);
            return CreatedAtAction(nameof(GetApp), new { appName = createAppDto.Name }, null);
        }
        catch (AppNameTakenException)
        {
            return DistagsProblemResults.AppConflict(createAppDto.Name);
        }
    }

    [HttpDelete("{appName}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteApp(
        string appName,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            DeleteAppCommand command = new DeleteAppCommand { AppName = appName };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (UnknownAppNameException)
        {
            return DistagsProblemResults.AppNotFound(appName);
        }
    }
}
