using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly ContributionDtoConverter _DtoConverter;

    public ContributionsController(LevelTagDbContext dbContext, ContributionDtoConverter dtoConverter)
    {
        _DbContext = dbContext;
        _DtoConverter = dtoConverter;
    }

    [HttpGet]
    public async Task<IActionResult> GetContributions([FromQuery] GetContributionsParameters parameters)
    {
        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>();

        if (parameters.LevelId != null)
            contributions = contributions.Where(contribution => contribution.LevelId == parameters.LevelId);

        if (parameters.UserSteamId != null)
            contributions = contributions.Where(contribution => contribution.User!.SteamId == parameters.UserSteamId);

        contributions = contributions.OrderByDescending(contribution => contribution.ContributedOn);

        bool respectAnonymity = !User.IsInRole(Constants.Roles.Moderator);
        List<ContributionDto> contributionDtos = await _DtoConverter.SelectDto(contributions, respectAnonymity).ToListAsync();

        return Ok(contributionDtos);
    }
}