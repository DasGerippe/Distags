using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserDto> GetMe()
    {
        string? steamIdValue = User.FindFirstValue(Constants.Claims.SteamId);
        if (steamIdValue == null || !long.TryParse(steamIdValue, out long steamId))
            return Unauthorized();

        List<string> roles = User.Claims
            .Where(claim => claim.Type == ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList();

        UserDto userDto = new UserDto
        {
            SteamId = steamId,
            Roles = roles,
        };

        return Ok(userDto);
    }
}