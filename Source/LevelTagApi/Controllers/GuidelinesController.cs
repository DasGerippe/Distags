using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuidelinesController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IUserManager _UserManager;

    public GuidelinesController(LevelTagDbContext dbContext, IUserManager userManager)
    {
        _DbContext = dbContext;
        _UserManager = userManager;
    }

    [HttpGet("contribution")]
    public ActionResult<GuidelinesDto> GetContributionGuidelines()
    {
        GuidelinesDto guidelines = new GuidelinesDto
        {
            Guidelines =
            [
                new GuidelineDto
                {
                    Title = "Tag in Good Faith",
                    Text = "Only add or remove tags when you genuinely believe the change is accurate.",
                },
                new GuidelineDto
                {
                    Title = "Follow the Official Tag Definitions",
                    Text = "Please follow our official tag definitions and do not reinterpret tags based on personal opinions or alternate meanings.",
                },
                new GuidelineDto
                {
                    Title = "No Vandalism or Malicious Editing",
                    Text = "Knowingly removing correct tags or adding incorrect ones is considered vandalism and is not allowed. Honest mistakes are fine - intentional misuse is not.",
                },
                new GuidelineDto
                {
                    Title = "Consequences for Violations",
                    Text = "Serious violations of those guidelines may result in your contribution rights being revoked and/or your account being banned.",
                },
            ],
        };

        return Ok(guidelines);
    }

    [HttpPost("contribution/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptContributionGuidelines()
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized();

        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == user.SteamId)
            .FirstOrDefaultAsync();
        if (userBan != null)
            return Unauthorized("You have been banned.");

        if (!user.AcceptedContributionGuidelines)
        {
            user.AcceptedContributionGuidelines = true;
            if (!user.Roles.Contains(Constants.Roles.Tagger))
                user.Roles.Add(Constants.Roles.Tagger);

            await _DbContext.SaveChangesAsync();
        }

        return NoContent();
    }
}