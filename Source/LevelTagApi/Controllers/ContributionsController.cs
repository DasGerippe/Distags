using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos.Contributions;
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

    public ContributionsController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<GetContributionsResponse>> GetContributions([FromQuery] GetContributionsParameters parameters,
        [FromServices] IQueryableFilterFactory<Contribution, ContributionFilterParameters> contributionFilterFactory,
        [FromServices] IDtoConverterFactory<Contribution, ContributionDto, ContributionConversionParameters> dtoConverterFactory)
    {
        bool ignoreAnonymity = User.IsInRole(Constants.Roles.Moderator);

        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>()
            .Where(contribution => !contribution.IsReverted && contribution.Changes.Any());

        ContributionFilterParameters filterParams = GetFilterParameters(parameters, ignoreAnonymity);
        IQueryableFilter<Contribution> contributionFilter = contributionFilterFactory.GetQueryableFilter(filterParams);
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

        ContributionConversionParameters conversionParameters = GetConversionParameters(parameters, ignoreAnonymity);
        IDtoConverter<Contribution, ContributionDto> dtoConverter = dtoConverterFactory.GetDtoConverter(conversionParameters);
        List<ContributionDto> contributionDtos = await dtoConverter.SelectDto(contributions).ToListAsync();

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

    private ContributionFilterParameters GetFilterParameters(GetContributionsParameters parameters, bool ignoreAnonymity)
    {
        return new ContributionFilterParameters
        {
            UserSteamId = parameters.UserSteamId,
            LevelId = parameters.LevelId,
            TagName = parameters.TagName,
            IgnoreAnonymity = ignoreAnonymity,
        };
    }

    private ContributionConversionParameters GetConversionParameters(GetContributionsParameters parameters, bool ignoreAnonymity)
    {
        return new ContributionConversionParameters
        {
            IgnoreAnonymity = ignoreAnonymity,
            IncludeLevel = parameters.LevelId == null,
            IncludeContributor = parameters.UserSteamId == null,
        };
    }
}