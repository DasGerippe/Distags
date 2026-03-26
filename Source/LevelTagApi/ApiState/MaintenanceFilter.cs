using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LevelTagApi.ApiState;

public class MaintenanceFilter : IAsyncActionFilter
{
    private readonly IApiStateService _ApiStateService;

    public MaintenanceFilter(IApiStateService apiStateService)
    {
        _ApiStateService = apiStateService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        bool maintenanceActive = await _ApiStateService.GetMaintenanceStateAsync();
        if (!maintenanceActive)
        {
            await next();
            return;
        }

        ClaimsPrincipal contextUser = context.HttpContext.User;
        if (contextUser.IsInRole(Constants.Roles.Moderator) || contextUser.IsInRole(Constants.Roles.Administrator))
        {
            await next();
            return;
        }

        context.Result = new ObjectResult(new
        {
            error = "Maintenance Mode",
            message = "This operation is temporarily unavailable.",
        })
        {
            StatusCode = StatusCodes.Status503ServiceUnavailable,
        };
    }
}