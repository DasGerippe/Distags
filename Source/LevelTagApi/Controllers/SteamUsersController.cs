using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SteamUsersController : ControllerBase
{
    private readonly LevelTagDbContext _LevelTagDbContext;

    public SteamUsersController(LevelTagDbContext levelTagDbContext)
    {
        _LevelTagDbContext = levelTagDbContext;
    }

    [HttpPut("batch")]
    public async Task<IActionResult> PutSteamUsers([FromBody] List<SteamUserDto> steamUserDtos)
    {
        List<long> steamUserIds = steamUserDtos.Select(steamUserDto => steamUserDto.Id).ToList();
        List<SteamUser> existingSteamUsers = await _LevelTagDbContext.SteamUsers
            .Where(steamUser => steamUserIds.Contains(steamUser.Id))
            .ToListAsync();

        foreach (SteamUserDto steamUserDto in steamUserDtos)
        {
            SteamUser? steamUser = existingSteamUsers.FirstOrDefault(steamUser => steamUser.Id == steamUserDto.Id);
            if (steamUser == null)
            {
                steamUser = new SteamUser();
                steamUser.Id = steamUserDto.Id;

                _LevelTagDbContext.SteamUsers.Add(steamUser);
            }
            steamUser.Name = steamUserDto.Name;
        }

        await _LevelTagDbContext.SaveChangesAsync();

        return NoContent();
    }
}