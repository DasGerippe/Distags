using Distags.Api.Contracts.UserBans;
using Distags.Api.Mapping;
using Distags.Api.Security;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.BanSteamId;
using Distags.Core.Commands.UnbanSteamId;
using Distags.Core.Exceptions;
using Distags.Core.Queries.UserBanBySteamId;
using Distags.Core.Queries.UserBans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserBansController : ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name, Roles = UserRoles.Moderator)]
    public async Task<ActionResult<List<UserBanResponse>>> GetUserBans(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<UserBanDto> userBanDtos = await requestDispatcher.DispatchAsync(new UserBansQuery());
        List<UserBanResponse> userBanResponses = userBanDtos
            .Select(UserBanMapper.Map)
            .ToList();
        return Ok(userBanResponses);
    }

    [HttpGet("{steamId:long}")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name, Roles = UserRoles.Moderator)]
    public async Task<ActionResult<UserBanResponse>> GetUserBan(
        long steamId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBanBySteamIdQuery query = new UserBanBySteamIdQuery { SteamId = steamId };
        UserBanDto? userBanDto = await requestDispatcher.DispatchAsync(query);

        if (userBanDto == null)
            return ProblemDetailsResult.UserBanNotFound(steamId);

        UserBanResponse userBanResponse = UserBanMapper.Map(userBanDto);
        return Ok(userBanResponse);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name, Roles = UserRoles.Moderator)]
    public async Task<IActionResult> PostUserBan(
        [FromBody] CreateUserBanRequest userBanDto,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            BanSteamIdCommand command = new BanSteamIdCommand
            {
                SteamId = userBanDto.SteamId,
                BannerSteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value,
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
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name, Roles = UserRoles.Moderator)]
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
