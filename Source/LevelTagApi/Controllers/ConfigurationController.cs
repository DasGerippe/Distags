using Distags.Core.Commands;
using Distags.Core.Commands.SetMaintenance;
using Distags.Core.Queries;
using Distags.Core.Queries.Maintenance;
using LevelTagApi.Dtos.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    public async Task<ActionResult<ConfigurationDto>> GetConfiguration(
        [FromServices] IQueryHandler<MaintenanceQuery, bool> maintenanceQueryHandler)
    {
        ConfigurationDto configuration = new ConfigurationDto
        {
            MaintenanceActive = await maintenanceQueryHandler.HandleAsync(new MaintenanceQuery()),
        };

        return Ok(configuration);
    }

    [HttpPatch]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> PatchConfiguration([FromBody] PatchConfigurationParameters apiSettings,
        [FromServices] ICommandHandler<SetMaintenanceCommand> setMaintenanceCommandHandler)
    {
        if (apiSettings.MaintenanceActive != null)
        {
            SetMaintenanceCommand command = new SetMaintenanceCommand { MaintenanceActive = apiSettings.MaintenanceActive };
            await setMaintenanceCommandHandler.HandleAsync(command);
        }

        return NoContent();
    }
}