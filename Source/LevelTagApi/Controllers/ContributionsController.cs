using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IContributionManager _ContributionManager;
    private readonly ContributionDtoConverter _DtoConverter;

    public ContributionsController(
        LevelTagDbContext dbContext,
        IContributionManager contributionManager,
        ContributionDtoConverter dtoConverter)
    {
        _DbContext = dbContext;
        _ContributionManager = contributionManager;
        _DtoConverter = dtoConverter;
    }

    [HttpGet]
    public async Task<ActionResult<GetContributionsResponse>> GetContributions([FromQuery] GetContributionsParameters parameters)
    {
        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>()
            .Where(contribution => !contribution.IsReverted);

        if (parameters.LevelId != null)
            contributions = contributions.Where(contribution => contribution.LevelId == parameters.LevelId);

        if (parameters.UserSteamId != null)
            contributions = contributions.Where(contribution => contribution.User!.SteamId == parameters.UserSteamId);

        if (!string.IsNullOrEmpty(parameters.Cursor))
        {
            bool isLong = long.TryParse(parameters.Cursor, out long lastContributionId);
            if (isLong)
                contributions = contributions.Where(contribution => contribution.Id < lastContributionId);
        }

        contributions = contributions
            .OrderByDescending(contribution => contribution.Id)
            .Take(parameters.Limit);

        bool respectAnonymity = !User.IsInRole(Constants.Roles.Moderator);
        List<ContributionDto> contributionDtos = await _DtoConverter.SelectDto(contributions, respectAnonymity).ToListAsync();

        GetContributionsResponse response = new GetContributionsResponse
        {
            Contributions = contributionDtos,
            NextCursor = contributionDtos.LastOrDefault()?.Id.ToString(),
        };

        return Ok(response);
    }

    [HttpPost("revert")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> RevertContributions([FromBody] RevertContributionsParameters parameters)
    {
        long steamId = parameters.SteamId;
        DateTime revertStartDate = DateTime.SpecifyKind(parameters.RevertStartDate ?? DateTime.MinValue, DateTimeKind.Utc);

        RevertContributionsResponse response = await _ContributionManager.RevertContributionsAsync(steamId, revertStartDate);
        return Ok(response);
    }
}