using Distags.Api.Contracts.Contributions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.RevertContributions;
using Distags.Core.Queries.Contributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ContributionsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ContributionsPage>> GetContributions([FromQuery] GetContributionsParameters parameters,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        ContributionsQuery query = new ContributionsQuery
        {
            IgnoreAnonymity = User.IsInRole(UserRoles.Moderator),
            UserSteamId = parameters.UserSteamId,
            AppName = parameters.AppName,
            LevelId = parameters.LevelId,
            TagName = parameters.TagName,
            ContributedAfter = parameters.ContributedAfter,
            ContributedBefore = parameters.ContributedBefore,
            DescendingOrder = parameters.DescendingOrder,
            Limit = parameters.Limit,
        };
        ContributionsPage contributionsPage = await requestDispatcher.DispatchAsync(query);
        return Ok(contributionsPage);
    }

    [HttpPost("revert")]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<IActionResult> RevertContributions(
        [FromBody] RevertContributionsParameters parameters,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        RevertContributionsCommand command = new RevertContributionsCommand
        {
            UserSteamId = parameters.SteamId,
            RevertStartDate = parameters.RevertStartDate,
        };
        await requestDispatcher.DispatchAsync(command);
        return NoContent();
    }
}