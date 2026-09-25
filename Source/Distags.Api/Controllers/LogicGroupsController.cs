using Distags.Api.Contracts.LogicGroups;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.LogicGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LogicGroupsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<LogicGroupResponse>>> GetLogicGroupsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<LogicGroupDto> logicGroupDtos = await requestDispatcher.DispatchAsync(new LogicGroupsQuery());
        List<LogicGroupResponse> logicGroupResponses = logicGroupDtos
            .Select(LogicGroupMapper.Map)
            .ToList();
        return Ok(logicGroupResponses);
    }
}
