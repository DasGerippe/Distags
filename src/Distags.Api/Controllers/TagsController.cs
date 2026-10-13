using Distags.Api.Contracts.Tags;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TagResponse>>> GetTagsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<TagDto> tagDtos = await requestDispatcher.DispatchAsync(new TagsQuery());
        List<TagResponse> tagResponses = tagDtos
            .Select(TagMapper.Map)
            .ToList();
        return Ok(tagResponses);
    }
}
