using Distags.Core.Commands;
using Distags.Core.Commands.BanSteamId;
using Distags.Core.Commands.UnbanSteamId;
using Distags.Core.Exceptions;
using Distags.Core.Queries;
using Distags.Core.Queries.UserBanBySteamId;
using Distags.Core.Queries.UserBans;
using LevelTagApi.Dtos.UserBans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserBanDto = Distags.Contracts.UserBanDto;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserBansController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<List<UserBanDto>>> GetUserBans(
        [FromServices] IQueryHandler<UserBansQuery, List<UserBanDto>> userBansQueryHandler)
    {
        List<UserBanDto> userBanDtos = await userBansQueryHandler.HandleAsync(new UserBansQuery());
        return Ok(userBanDtos);
    }

    [HttpGet("{steamId:long}")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<ActionResult<UserBanDto>> GetUserBan(long steamId,
        [FromServices] UserBanBySteamIdQueryHandler userBanBySteamIdQueryHandler)
    {
        UserBanBySteamIdQuery query = new UserBanBySteamIdQuery { SteamId = steamId };
        UserBanDto? userBanDto = await userBanBySteamIdQueryHandler.HandleAsync(query);

        return userBanDto != null
            ? Ok(userBanDto)
            : NotFound("The Steam ID is not banned.");
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> PostUserBan([FromBody] CreateUserBanDto userBanDto,
        [FromServices] ICommandHandler<BanSteamIdCommand> banSteamIdCommandHandler)
    {
        try
        {
            Claim? steamIdClaim = User.FindFirst(Constants.Claims.SteamId);
            if (steamIdClaim == null || !long.TryParse(steamIdClaim.Value, out long bannerSteamId))
                return Unauthorized();

            BanSteamIdCommand command = new BanSteamIdCommand
            {
                SteamId = userBanDto.SteamId,
                BannerSteamId = bannerSteamId,
                Reason = userBanDto.Reason,
            };
            await banSteamIdCommandHandler.HandleAsync(command);

            return CreatedAtAction(nameof(GetUserBan), new { steamId = userBanDto.SteamId }, null);
        }
        catch (UnknownUserException)
        {
            return Unauthorized();
        }
        catch (CannotBanSelfException)
        {
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                detail: "You don't have permission to ban yourself.");
        }
        catch (CannotBanOtherUserException)
        {
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                detail: "You don't have permission to ban that Steam ID.");
        }
        catch (SteamIdAlreadyBannedException)
        {
            return Conflict("The Steam ID is already banned.");
        }
    }

    [HttpDelete("{steamId:long}")]
    [Authorize(Roles = Constants.Roles.Moderator)]
    public async Task<IActionResult> DeleteUserBan(long steamId,
        [FromServices] ICommandHandler<UnbanSteamIdCommand> unbanSteamIdCommandHandler)
    {
        try
        {
            UnbanSteamIdCommand command = new UnbanSteamIdCommand { SteamId = steamId };
            await unbanSteamIdCommandHandler.HandleAsync(command);

            return NoContent();
        }
        catch (SteamIdNotBannedException)
        {
            return NotFound("The Steam ID is not banned.");
        }
    }
}