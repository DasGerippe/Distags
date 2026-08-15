using Distags.Api.Contracts.Configuration;
using Distags.Core;
using Distags.Core.Commands.SetMaintenance;
using Distags.Core.Queries.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    public async Task<ActionResult<ConfigurationDto>> GetConfiguration(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        ConfigurationDto configuration = new ConfigurationDto
        {
            MaintenanceActive = await requestDispatcher.DispatchAsync(new MaintenanceQuery()),
        };

        return Ok(configuration);
    }

    [HttpPatch]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> PatchConfiguration([FromBody] PatchConfigurationParameters apiSettings,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        if (apiSettings.MaintenanceActive != null)
        {
            SetMaintenanceCommand command = new SetMaintenanceCommand { MaintenanceActive = apiSettings.MaintenanceActive };
            await requestDispatcher.DispatchAsync(command);
        }

        return NoContent();
    }
}
