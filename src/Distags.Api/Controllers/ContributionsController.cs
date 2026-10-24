using Distags.Api.Contracts.Contributions;
using Distags.Api.Mapping;
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
    public async Task<ActionResult<ContributionsPageResponse>> GetContributions(
        [FromQuery] GetContributionsRequest request,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        ContributionsQuery query = new ContributionsQuery
        {
            IgnoreAnonymity = User.IsInRole(UserRoles.Moderator),
            UserSteamId = request.UserSteamId,
            AutoTaggerGuid = request.AutoTaggerGuid,
            LevelSlug = request.LevelSlug,
            LevelWorkshopId = request.LevelWorkshopId,
            TagName = request.TagName,
            ContributedAfter = request.ContributedAfter,
            ContributedBefore = request.ContributedBefore,
            DescendingOrder = request.DescendingOrder,
            Limit = request.Limit,
        };
        ContributionsPage contributionsPage = await requestDispatcher.DispatchAsync(query);
        ContributionsPageResponse contributionsPageResponse = ContributionsPageMapper.Map(contributionsPage);
        return Ok(contributionsPageResponse);
    }

    [HttpPost("revert")]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<IActionResult> RevertContributions(
        [FromBody] RevertContributionsRequest request,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        RevertContributionsCommand command = new RevertContributionsCommand
        {
            UserSteamId = request.SteamId,
            RevertStartDate = request.RevertStartDate,
        };
        await requestDispatcher.DispatchAsync(command);
        return NoContent();
    }
}
