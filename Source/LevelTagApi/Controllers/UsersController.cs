using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IUserManager _UserManager;

    public UsersController(LevelTagDbContext dbContext, IUserManager userManager)
    {
        _DbContext = dbContext;
        _UserManager = userManager;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized();

        UserDto userDto = user.ToDto();
        return Ok(userDto);
    }

    [HttpPatch("me/settings")]
    [Authorize]
    public async Task<IActionResult> PatchSettings([FromBody] PatchSettingsParameters parameters)
    {
        User? user = await _UserManager.GetCurrentUserAsync();
        if (user == null)
            return Unauthorized();

        user.IsAnonymous = parameters.IsAnonymous ?? user.IsAnonymous;
        
        await _DbContext.SaveChangesAsync();
        return NoContent();
    }
}