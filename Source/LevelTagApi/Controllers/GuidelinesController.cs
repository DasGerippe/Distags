using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<ActionResult<GuidelinesDto?>> GetContributionGuidelines()
    {
        await using Stream fileStream = new FileStream("contribution-guidelines.json", FileMode.Open, FileAccess.Read);
        GuidelinesDto? guidelinesDto = await JsonSerializer.DeserializeAsync<GuidelinesDto>(fileStream);
        return Ok(guidelinesDto);
    }

    [HttpPost("contribution/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptContributionGuidelines()
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized();

        if (user.IsBanned)
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