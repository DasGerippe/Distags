using Distags.Api.Contracts.AutoTaggers;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = Distags.Contracts.Constants.UserRoles.Administrator)]
public class AutoTaggersController : ControllerBase
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public AutoTaggersController(IRequestDispatcher requestDispatcher)
    {
        _RequestDispatcher = requestDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<List<AutoTaggerResponse>>> GetAutoTaggers()
    {
        List<AutoTaggerDto> autoTaggerDtos = await _RequestDispatcher.DispatchAsync(new AutoTaggersQuery());
        List<AutoTaggerResponse> autoTaggerResponses = autoTaggerDtos
            .Select(AutoTaggerMapper.Map)
            .ToList();
        return autoTaggerResponses;
    }
}
