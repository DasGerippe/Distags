using Distags.Api.Contracts.DisplayGroups;
using Distags.Api.Mapping;
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
    public async Task<ActionResult<List<DisplayGroupResponse>>> GetDisplayGroupsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<DisplayGroupDto> displayGroupDtos = await requestDispatcher.DispatchAsync(new DisplayGroupsQuery());
        List<DisplayGroupResponse> displayGroupResponses = displayGroupDtos
            .Select(DisplayGroupMapper.Map)
            .ToList();
        return Ok(displayGroupResponses);
    }
}
