using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserBansController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;

    public UserBansController(LevelTagDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<List<UserBanDto>>> GetUserBans()
    {
        List<UserBanDto> userBanDtos = await _DbContext.Set<UserBan>()
            .GroupJoin(
                _DbContext.Set<User>(),
                userBan => userBan.SteamId,
                user => user.SteamId,
                (userBan, users) => new { UserBan = userBan, Users = users })
            .SelectMany(
                userBans => userBans.Users.DefaultIfEmpty(),
                (joinedUserBan, user) => new UserBanDto
                {
                    SteamId = joinedUserBan.UserBan.SteamId,
                    SteamName = user!.SteamName,
                    SteamAvatarHash = user!.SteamAvatarHash,
                    BannedOn = joinedUserBan.UserBan.BannedOn,
                    Reason = joinedUserBan.UserBan.Reason,
                })
            .OrderByDescending(userBanDto => userBanDto.BannedOn)
            .ToListAsync();

        return Ok(userBanDtos);
    }

    [HttpGet("{steamId:long}")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<UserBanDto>> GetUserBan(long steamId)
    {
        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == steamId)
            .FirstOrDefaultAsync();

        return userBan != null
            ? Ok(userBan)
            : NotFound("The Steam ID is not banned.");
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> PostUserBan([FromBody] CreateUserBanDto userBanDto)
    {
        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == userBanDto.SteamId)
            .FirstOrDefaultAsync();

        if (userBan != null)
            return Conflict("The Steam ID is already banned.");

        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == userBanDto.SteamId)
            .FirstOrDefaultAsync();

        if (user != null)
            user.Roles = [];

        userBan = new UserBan
        {
            SteamId = userBanDto.SteamId,
            BannedOn = DateTime.UtcNow,
            Reason = userBanDto.Reason,
        };
        _DbContext.Add(userBan);
        await _DbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserBan), new { steamId = userBan.SteamId }, null);
    }

    [HttpDelete("{steamId:long}")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> DeleteUserBan(long steamId)
    {
        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == steamId)
            .FirstOrDefaultAsync();

        if (userBan == null)
            return NotFound("The Steam ID is not banned.");

        _DbContext.Remove(userBan);
        await _DbContext.SaveChangesAsync();

        return NoContent();
    }
}