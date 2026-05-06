using Distags.Contracts;
using Distags.Core.Commands;
using Distags.Core.Commands.UpdateUserSettings;
using Distags.Core.Exceptions;
using Distags.Core.Queries;
using Distags.Core.Queries.UserBySteamId;
using LevelTagApi.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserDto = Distags.Contracts.UserDto;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe(
        [FromServices] IQueryHandler<UserBySteamIdQuery, UserDto?> userBySteamIdQueryHandler)
    {
        Claim? steamIdClaim = User.FindFirst(Constants.Claims.SteamId);
        if (steamIdClaim == null || !long.TryParse(steamIdClaim.Value, out long steamId))
            return Unauthorized();

        UserDto? user = await userBySteamIdQueryHandler.HandleAsync(new UserBySteamIdQuery { SteamId = steamId });
        return user == null
            ? Unauthorized()
            : Ok(user);
    }

    [HttpPatch("me/settings")]
    [Authorize]
    public async Task<IActionResult> PatchSettings([FromBody] PatchUserSettingsParameters parameters,
        [FromServices] ICommandHandler<UpdateUserSettingsCommand> updateUserSettingsCommandHandler)
    {
        try
        {
            Claim? steamIdClaim = User.FindFirst(Constants.Claims.SteamId);
            if (steamIdClaim == null || !long.TryParse(steamIdClaim.Value, out long steamId))
                return Unauthorized();

            UpdateUserSettingsCommand command = new UpdateUserSettingsCommand
            {
                SteamId = steamId,
                AcceptsGuidelines = parameters.AcceptsGuidelines?.Value ?? default(UpdateValue<bool>),
                IsAnonymous = parameters.IsAnonymous?.Value ?? default(UpdateValue<bool>),
            };

            await updateUserSettingsCommandHandler.HandleAsync(command);

            return NoContent();
        }
        catch (UnknownUserException)
        {
            return Unauthorized();
        }
    }
}