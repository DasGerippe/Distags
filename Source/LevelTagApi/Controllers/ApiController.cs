using LevelTagApi.ApiState;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : Controller
{
    private readonly IApiStateService _ApiStateService;

    public ApiController(IApiStateService apiStateService)
    {
        _ApiStateService = apiStateService;
    }

    [HttpGet("state")]
    public async Task<ActionResult<ApiStateDto>> GetState()
    {
        return new ApiStateDto
        {
            MaintenanceActive = await _ApiStateService.GetMaintenanceStateAsync(),
        };
    }

    [HttpPatch("state")]
    [Authorize(Roles = Constants.Roles.Administrator)]
    public async Task<IActionResult> PatchState([FromBody] PatchApiStateParameters apiSettings)
    {
        if (apiSettings.MaintenanceActive != null)
            await _ApiStateService.SetMaintenanceStateAsync(apiSettings.MaintenanceActive);

        return NoContent();
    }
}