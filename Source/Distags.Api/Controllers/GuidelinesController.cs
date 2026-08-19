using Distags.Api.Contracts.Guidelines;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GuidelinesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GuidelinesDto?>> GetContributionGuidelines()
    {
        await using Stream fileStream = new FileStream("contribution-guidelines.json", FileMode.Open, FileAccess.Read);
        GuidelinesDto? guidelinesDto = await JsonSerializer.DeserializeAsync<GuidelinesDto>(fileStream);
        return Ok(guidelinesDto);
    }
}
