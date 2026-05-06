using Distags.Contracts;
using Distags.Core.Commands;
using Distags.Core.Commands.RevertContributions;
using Distags.Core.Queries;
using Distags.Core.Queries.Contributions;
using LevelTagApi.Dtos.Contributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ContributionsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ContributionsPage>> GetContributions([FromQuery] GetContributionsParameters parameters,
        [FromServices] IQueryHandler<ContributionsQuery, ContributionsPage> contributionsQueryHandler)
    {
        ContributionsQuery query = new ContributionsQuery
        {
            IgnoreAnonymity = User.IsInRole(Constants.Roles.Moderator),
            UserSteamId = parameters.UserSteamId,
            AppName = parameters.AppName,
            LevelId = parameters.LevelId,
            TagName = parameters.TagName,
            Limit = parameters.Limit,
            Cursor = parameters.Cursor,
        };
        ContributionsPage contributionsPage = await contributionsQueryHandler.HandleAsync(query);
        return Ok(contributionsPage);
    }

    [HttpPost("revert")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> RevertContributions(
        [FromBody] RevertContributionsParameters parameters,
        [FromServices] ICommandHandler<RevertContributionsCommand> revertContributionsCommandHandler)
    {
        RevertContributionsCommand command = new RevertContributionsCommand
        {
            UserSteamId = parameters.SteamId,
            RevertStartDate = parameters.RevertStartDate,
        };
        await revertContributionsCommandHandler.HandleAsync(command);
        return NoContent();
    }
}