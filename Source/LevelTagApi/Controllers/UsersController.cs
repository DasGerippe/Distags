using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public UsersController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        User? user = await _DbContext.GetUser(User);
        if (user == null)
            return Unauthorized();

        UserDto userDto = user.ToDto();
        return Ok(userDto);
    }

    [HttpPatch("me/settings")]
    [Authorize]
    public async Task<IActionResult> PatchSettings([FromBody] PatchSettingsParameters parameters)
    {
        User? user = await _DbContext.GetUser(User);
        if (user == null)
            return Unauthorized();

        user.IsAnonymous = parameters.IsAnonymous ?? user.IsAnonymous;

        return Ok();
    }
}