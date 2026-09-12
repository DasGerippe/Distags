using Distags.Api.Contracts.Users;
using Distags.Api.Mapping;
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
    public async Task<ActionResult<UserResponse>> GetMe(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBySteamIdQuery query = new UserBySteamIdQuery
        {
            SteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value,
        };
        UserDto? userDto = await requestDispatcher.DispatchAsync(query);
        if (userDto == null)
            return Unauthorized();

        UserResponse userResponse = UserMapper.Map(userDto);
        return Ok(userResponse);
    }

    [HttpPatch("me/settings")]
    [Authorize(AuthenticationSchemes = Constants.Authentication.SessionTokenScheme.Name)]
    public async Task<IActionResult> PatchSettings(
        [FromBody] PatchUserSettingsRequest request,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        try
        {
            UpdateUserSettingsCommand command = new UpdateUserSettingsCommand
            {
                SteamId = User.FindFirstValue<long>(Constants.Claims.SteamId)!.Value,
                AcceptsGuidelines = request.AcceptsGuidelines?.Value ?? default(UpdateValue<bool>),
                IsAnonymous = request.IsAnonymous?.Value ?? default(UpdateValue<bool>),
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
    public async Task<ActionResult<UserResponse>> GetUser(
        long userSteamId,
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        UserBySteamIdQuery query = new UserBySteamIdQuery { SteamId = userSteamId };
        UserDto? userDto = await requestDispatcher.DispatchAsync(query);
        if (userDto == null)
            return ProblemDetailsResult.UserNotFound(userSteamId);

        UserResponse userResponse = UserMapper.Map(userDto);
        return Ok(userResponse);
    }

    [HttpPut("{userSteamId:long}/roles/{role}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> AddUserRole(
        long userSteamId,
        string role,
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
    public async Task<IActionResult> RemoveUserRole(
        long userSteamId,
        string role,
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
