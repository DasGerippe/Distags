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
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTagsAsync(
        [FromServices] IRequestDispatcher requestDispatcher)
    {
        List<TagDto> tagDtos = await requestDispatcher.DispatchAsync(new TagsQuery());
        return Ok(tagDtos);
    }
}
