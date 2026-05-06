using Distags.Contracts;
using Distags.Core.Queries;
using Distags.Core.Queries.TagTree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelTagApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController : ControllerBase
{
    [HttpGet("tree")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TagGroupDto>>> GetTagTreeAsync(
        [FromServices] IQueryHandler<TagTreeQuery, List<TagGroupDto>> tagTreeQueryHandler)
    {
        List<TagGroupDto> tagGroupDtos = await tagTreeQueryHandler.HandleAsync(new TagTreeQuery());
        return Ok(tagGroupDtos);
    }
}