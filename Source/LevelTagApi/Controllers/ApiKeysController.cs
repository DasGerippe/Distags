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

    [HttpGet("{apiKeyValue}")]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<ActionResult<ApiKeyDto>> GetApiKey(string apiKeyValue)
    {
        ApiKey? apiKey = await _DbContext.Set<ApiKey>().FirstOrDefaultAsync(apiKey => apiKey.Value == apiKeyValue);
        if (apiKey == null)
            return NotFound();

        ApiKeyDto apiKeyDto = apiKey.ToDto();
        return Ok(apiKeyDto);
    }

    [HttpPost]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<ActionResult<ApiKeyDto>> CreateApiKey([FromBody] PatchApiKeyParameters? parameters)
    {
        string apiKeyValue = _TokenGenerator.GenerateToken(32);

        ApiKey apiKey = new ApiKey
        {
            Value = apiKeyValue,
        };

        if (parameters != null)
            PatchApiKey(apiKey, parameters);

        _DbContext.Add(apiKey);
        await _DbContext.SaveChangesAsync();

        ApiKeyDto apiKeyDto = apiKey.ToDto();
        return CreatedAtAction(nameof(GetApiKey), new { apiKeyValue }, apiKeyDto);
    }

    [HttpPatch("{apiKeyValue}")]
    [Authorize(Roles = Constants.Roles.ApiKeyManager)]
    public async Task<IActionResult> PatchApiKey(string apiKeyValue, [FromBody] PatchApiKeyParameters parameters)
    {
        ApiKey? apiKey = await _DbContext.Set<ApiKey>().FirstOrDefaultAsync(apiKey => apiKey.Value == apiKeyValue);
        if (apiKey == null)
            return NotFound();

        PatchApiKey(apiKey, parameters);

        await _DbContext.SaveChangesAsync();
        return NoContent();
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

    private void PatchApiKey(ApiKey apiKey, PatchApiKeyParameters parameters)
    {
        apiKey.Comment = parameters.Comment ?? apiKey.Comment;
    }
}