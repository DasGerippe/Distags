using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<UserDto>> GetMe([FromHeader(Name = LevelTagApiConstants.SessionTokenHeader)] string sessionToken)
    {
        UserSession? userSession = await _DbContext.Set<UserSession>()
            .Include(session => session.User)
            .FirstOrDefaultAsync(session => session.Token == sessionToken);

        if (userSession?.User.SteamId == null)
            return Unauthorized();

        UserDto userDto = userSession.User.ToDto();
        return Ok(userDto);
    }
}