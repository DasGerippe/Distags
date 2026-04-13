using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos;
using LevelTagApi.Filtering;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IQueryableFilterFactory<Contribution, ContributionFilterParameters> _ContributionFilterFactory;
    private readonly IContributionManager _ContributionManager;
    private readonly ContributionDtoConverter _DtoConverter;

    public ContributionsController(
        LevelTagDbContext dbContext,
        IQueryableFilterFactory<Contribution, ContributionFilterParameters> contributionFilterFactory,
        IContributionManager contributionManager,
        ContributionDtoConverter dtoConverter)
    {
        _DbContext = dbContext;
        _ContributionFilterFactory = contributionFilterFactory;
        _ContributionManager = contributionManager;
        _DtoConverter = dtoConverter;
    }

    [HttpGet]
    public async Task<ActionResult<GetContributionsResponse>> GetContributions([FromQuery] GetContributionsParameters parameters)
    {
        bool respectAnonymity = !User.IsInRole(Constants.Roles.Moderator);

        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>()
            .Where(contribution => !contribution.IsReverted);

        ContributionFilterParameters filterParams = GetFilterParameters(parameters, respectAnonymity);
        IQueryableFilter<Contribution> contributionFilter = _ContributionFilterFactory.GetQueryableFilter(filterParams);
        contributions = contributionFilter.Filter(contributions);

        if (!string.IsNullOrEmpty(parameters.Cursor))
        {
            bool isLong = long.TryParse(parameters.Cursor, out long lastContributionId);
            if (isLong)
                contributions = contributions.Where(contribution => contribution.Id < lastContributionId);
        }

        contributions = contributions
            .OrderByDescending(contribution => contribution.Id)
            .Take(parameters.Limit);

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
    public async Task<IActionResult> RevertContributions(
        [FromBody] RevertContributionsParameters parameters,
        [FromServices] IContributionReverter contributionReverter)
    {
        long steamId = parameters.SteamId;
        DateTimeOffset revertStartDate = parameters.RevertStartDate ?? DateTimeOffset.MinValue;

        await contributionReverter.RevertContributionsAsync(steamId, revertStartDate);
        return NoContent();
    }

    private ContributionFilterParameters GetFilterParameters(GetContributionsParameters parameters, bool respectAnonymity)
    {
        return new ContributionFilterParameters
        {
            UserSteamId = parameters.UserSteamId,
            LevelId = parameters.LevelId,
            TagName = parameters.Tag,
            RespectAnonymity = respectAnonymity,
        };
    }
}