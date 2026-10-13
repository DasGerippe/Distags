using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.DisplayGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DisplayGroupsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<DisplayGroupDto>> GetDisplayGroupsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<DisplayGroupDto> displayGroupsDtos = await requestDispatcher.DispatchAsync(new DisplayGroupsQuery());
        return displayGroupsDtos;
    }
}
