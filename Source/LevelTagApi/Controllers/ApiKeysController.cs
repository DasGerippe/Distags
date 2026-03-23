using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly LevelTagDbContext _DbContext;
    private readonly ITokenGenerator _TokenGenerator;

    public ApiKeysController(LevelTagDbContext dbContext, ITokenGenerator tokenGenerator)
    {
        _DbContext = dbContext;
        _TokenGenerator = tokenGenerator;
    }

    [HttpGet]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<ActionResult<List<ApiKeyDto>>> GetApiKeys()
    {
        List<ApiKeyDto> apiKeys = await _DbContext.Set<ApiKey>()
            .Select(apiKey => apiKey.ToDto())
            .ToListAsync();
        return Ok(apiKeys);
    }

    [HttpPost("create")]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<ActionResult<ApiKeyDto>> CreateApiKey()
    {
        string apiKeyValue = _TokenGenerator.GenerateToken(32);
        
        ApiKey apiKey = new ApiKey
        {
            Value = apiKeyValue,
        };

        _DbContext.Add(apiKey);
        await _DbContext.SaveChangesAsync();

        ApiKeyDto apiKeyDto = apiKey.ToDto();
        return Ok(apiKeyDto);
    }

    [HttpDelete("{apiKeyValue}")]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<IActionResult> DeleteApiKey(string apiKeyValue)
    {
        ApiKey? apiKey = await _DbContext.Set<ApiKey>().FirstOrDefaultAsync(apiKey => apiKey.Value == apiKeyValue);
        if (apiKey == null)
            return NotFound();

        _DbContext.Remove(apiKey);
        await _DbContext.SaveChangesAsync();

        return Ok();
    }
}