using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContributionsController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public ContributionsController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetContributions([FromQuery] GetContributionsParameters parameters)
    {
        IQueryable<Contribution> contributions = _DbContext.Set<Contribution>();

        if (parameters.LevelId != null)
            contributions = contributions.Where(contribution => contribution.LevelId == parameters.LevelId);

        if (parameters.UserSteamId != null)
            contributions = contributions.Where(contribution => contribution.User!.SteamId == parameters.UserSteamId);

        contributions = contributions
            .OrderByDescending(contribution => contribution.ContributedOn)
            .Include(contribution => contribution.Level.Creator)
            .Include(contribution => contribution.Tag)
            .Include(contribution => contribution.User)
            .Include(contribution => contribution.App);

        bool respectAnonymity = User.IsInRole(Constants.Roles.Moderator);
        List<ContributionDto> contributionDtos = await contributions
            .Select(contribution => contribution.ToDto(!respectAnonymity))
            .ToListAsync();

        return Ok(contributionDtos);
    }
}