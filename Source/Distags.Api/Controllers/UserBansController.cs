using Distags.Api.Contracts.UserBans;
using Distags.Core;
using Distags.Core.Commands.BanSteamId;
using Distags.Core.Commands.UnbanSteamId;
using Distags.Core.Exceptions;
using Distags.Core.Queries.UserBanBySteamId;
using Distags.Core.Queries.UserBans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Distags.Contracts.Constants;
using UserBanDto = Distags.Contracts.UserBanDto;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserBansController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<ActionResult<List<UserBanDto>>> GetUserBans(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<UserBanDto> userBanDtos = await requestDispatcher.DispatchAsync(new UserBansQuery());
        return Ok(userBanDtos);
    }

    [HttpGet("{steamId:long}")]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<ActionResult<UserBanDto>> GetUserBan(long steamId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBanBySteamIdQuery query = new UserBanBySteamIdQuery { SteamId = steamId };
        UserBanDto? userBanDto = await requestDispatcher.DispatchAsync(query);

        return userBanDto != null
            ? Ok(userBanDto)
            : ProblemDetailsResult.UserBanNotFound(steamId);
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<IActionResult> PostUserBan([FromBody] CreateUserBanDto userBanDto,
        [FromServices] IRequestDispatcher requestDispatcher)
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
            await requestDispatcher.DispatchAsync(command);

            return CreatedAtAction(nameof(GetUserBan), new { steamId = userBanDto.SteamId }, null);
        }
        catch (UnknownUserException)
        {
            return Unauthorized();
        }
        catch (CannotBanSelfException)
        {
            return ProblemDetailsResult.ForbiddenBanSelf();
        }
        catch (CannotBanOtherUserException)
        {
            return ProblemDetailsResult.ForbiddenBanOtherUser();
        }
        catch (SteamIdAlreadyBannedException)
        {
            return ProblemDetailsResult.UserBanConflict(userBanDto.SteamId);
        }
    }

    [HttpDelete("{steamId:long}")]
    [Authorize(Roles = UserRoles.Moderator)]
    public async Task<IActionResult> DeleteUserBan(long steamId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            UnbanSteamIdCommand command = new UnbanSteamIdCommand { SteamId = steamId };
            await requestDispatcher.DispatchAsync(command);

            return NoContent();
        }
        catch (SteamIdNotBannedException)
        {
            return ProblemDetailsResult.UserBanNotFound(steamId);
        }
    }
}