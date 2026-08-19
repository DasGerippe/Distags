using Distags.Api.Contracts.Users;
using Distags.Api.Security;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands;
using Distags.Core.Commands.AddUserRole;
using Distags.Core.Commands.RemoveUserRole;
using Distags.Core.Commands.UpdateUserSettings;
using Distags.Core.Exceptions;
using Distags.Core.Queries.UserBySteamId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<ActionResult<UserDto>> GetMe(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBySteamIdQuery query = new UserBySteamIdQuery
        {
            SteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value,
        };
        UserDto? user = await requestDispatcher.DispatchAsync(query);
        return user == null
            ? Unauthorized()
            : Ok(user);
    }

    [HttpPatch("me/settings")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> PatchSettings([FromBody] PatchUserSettingsParameters parameters,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            UpdateUserSettingsCommand command = new UpdateUserSettingsCommand
            {
                SteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value,
                AcceptsGuidelines = parameters.AcceptsGuidelines?.Value ?? default(UpdateValue<bool>),
                IsAnonymous = parameters.IsAnonymous?.Value ?? default(UpdateValue<bool>),
            };

            await requestDispatcher.DispatchAsync(command);

            return NoContent();
        }
        catch (UnknownUserException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("{userSteamId:long}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> GetUser(long userSteamId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBySteamIdQuery query = new UserBySteamIdQuery { SteamId = userSteamId };
        UserDto? user = await requestDispatcher.DispatchAsync(query);
        return user != null
            ? Ok(user)
            : ProblemDetailsResult.UserNotFound(userSteamId);
    }

    [HttpPut("{userSteamId:long}/roles/{role}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> AddUserRole(long userSteamId, string role,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            AddUserRoleCommand command = new AddUserRoleCommand
            {
                UserSteamId = userSteamId,
                Role = role,
            };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch (UnknownUserException)
        {
            return ProblemDetailsResult.UserNotFound(userSteamId);
        }
    }

    [HttpDelete("{userSteamId:long}/roles/{role}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> RemoveUserRole(long userSteamId, string role,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            RemoveUserRoleCommand command = new RemoveUserRoleCommand
            {
                UserSteamId = userSteamId,
                Role = role,
            };
            await requestDispatcher.DispatchAsync(command);
            return NoContent();
        }
        catch
        {
            return ProblemDetailsResult.UserNotFound(userSteamId);
        }
    }
}
