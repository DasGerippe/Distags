using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.LogicGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LogicGroupsController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<LogicGroupDto>> GetLogicGroupsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<LogicGroupDto> logicGroupDtos = await requestDispatcher.DispatchAsync(new LogicGroupsQuery());
        return logicGroupDtos;
    }
}
