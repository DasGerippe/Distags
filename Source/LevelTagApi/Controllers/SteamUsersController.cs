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
        List<Creator> existingSteamUsers = await _LevelTagDbContext.Creators
            .Where(steamUser => steamUserIds.Contains(steamUser.Id))
            .ToListAsync();

        foreach (SteamUserDto steamUserDto in steamUserDtos)
        {
            Creator? steamUser = existingSteamUsers.FirstOrDefault(steamUser => steamUser.Id == steamUserDto.Id);
            if (steamUser == null)
            {
                steamUser = new Creator();
                steamUser.Id = steamUserDto.Id;

                _LevelTagDbContext.Creators.Add(steamUser);
            }
            steamUser.Name = steamUserDto.Name;
        }

        await _LevelTagDbContext.SaveChangesAsync();

        return NoContent();
    }
}