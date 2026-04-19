using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos.UserBans;
using LevelTagApi.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserBansController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly IUserManager _UserManager;
    private readonly IUserBanPermissionChecker _PermissionChecker;
    private readonly UserBanDtoConverter _DtoConverter;

    public UserBansController(
        LevelTagDbContext dbContext,
        IUserManager userManager,
        IUserBanPermissionChecker permissionChecker,
        UserBanDtoConverter dtoConverter)
    {
        _DbContext = dbContext;
        _UserManager = userManager;
        _PermissionChecker = permissionChecker;
        _DtoConverter = dtoConverter;
    }

    [HttpGet]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<List<UserBanDto>>> GetUserBans()
    {
        List<UserBanDto> userBanDtos = await _DtoConverter.SelectDto(_DbContext.Set<UserBan>()).ToListAsync();

        return Ok(userBanDtos);
    }

    [HttpGet("{steamId:long}")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<UserBanDto>> GetUserBan(long steamId)
    {
        IQueryable<UserBan> userBanQuery = _DbContext.Set<UserBan>()
            .Include(userBan => userBan.Banner)
            .Where(userBan => userBan.SteamId == steamId);

        UserBanDto? userBanDto = await _DtoConverter.SelectDto(userBanQuery).FirstOrDefaultAsync();

        return userBanDto != null
            ? Ok(userBanDto)
            : NotFound("The Steam ID is not banned.");
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> PostUserBan([FromBody] CreateUserBanDto userBanDto)
    {
        User? banningUser = await _UserManager.GetCurrentUserAsync();
        if (banningUser == null)
            return Unauthorized();

        if (banningUser.SteamId == userBanDto.SteamId)
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                detail: "You don't have permission to ban yourself.");

        UserBan? userBan = await _DbContext.Set<UserBan>()
            .Where(userBan => userBan.SteamId == userBanDto.SteamId)
            .FirstOrDefaultAsync();

        if (userBan != null)
            return Conflict("The Steam ID is already banned.");

        User? bannedUser = await _DbContext.Set<User>()
            .Where(user => user.SteamId == userBanDto.SteamId)
            .FirstOrDefaultAsync();

        if (bannedUser != null)
        {
            if (!_PermissionChecker.CanBan(banningUser, bannedUser))
                return Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    detail: "You don't have permission to ban that Steam ID.");

            bannedUser.IsBanned = true;
            bannedUser.Roles = [];
        }

        userBan = new UserBan
        {
            SteamId = userBanDto.SteamId,
            BannedOn = DateTimeOffset.UtcNow,
            Banner = banningUser,
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

        User? user = await _DbContext.Set<User>()
            .Where(user => user.SteamId == steamId)
            .FirstOrDefaultAsync();
        user?.IsBanned = false;

        _DbContext.Remove(userBan);
        await _DbContext.SaveChangesAsync();

        return NoContent();
    }
}