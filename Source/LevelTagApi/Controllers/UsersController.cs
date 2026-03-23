using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public ActionResult<UserDto> GetMe()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Ok(new UserDto());

        string? steamIdValue = User.FindFirstValue(Constants.Claims.SteamId);
        if (steamIdValue == null || !long.TryParse(steamIdValue, out long steamId))
            return Ok(new UserDto());

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