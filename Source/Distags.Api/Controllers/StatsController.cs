using Distags.Api.Contracts.Stats;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.StatsSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatsController : ControllerBase
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public StatsController(IRequestDispatcher requestDispatcher)
    {
        _RequestDispatcher = requestDispatcher;
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    [OutputCache(PolicyName = Constants.Cache.IgnoreAuthPolicyName)]
    public async Task<ActionResult<StatsSummaryResponse>> GetStatsSummary()
    {
        StatsSummaryDto statsSummaryDto = await _RequestDispatcher.DispatchAsync(new StatsSummaryQuery());
        StatsSummaryResponse statsSummaryResponse = StatsSummaryMapper.Map(statsSummaryDto);
        return statsSummaryResponse;
    }
}
