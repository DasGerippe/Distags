using AutoTaggerWorker;
using AutoTagging.Abstractions;
using Distags.Api.Contracts.AutoTaggers;
using Distags.Api.Mapping;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Distags.Contracts.Constants;

namespace Distags.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AutoTaggersController : ControllerBase
{
    private readonly IRequestDispatcher _RequestDispatcher;

    public AutoTaggersController(IRequestDispatcher requestDispatcher)
    {
        _RequestDispatcher = requestDispatcher;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<AutoTaggerResponse>>> GetAutoTaggers()
    {
        List<AutoTaggerDto> autoTaggerDtos = await _RequestDispatcher.DispatchAsync(new AutoTaggersQuery());
        List<AutoTaggerResponse> autoTaggerResponses = autoTaggerDtos
            .Select(AutoTaggerMapper.Map)
            .ToList();
        return autoTaggerResponses;
    }

    [HttpPost("{autoTaggerName}/run")]
    [Authorize(Roles = UserRoles.Administrator)]
    public IActionResult RunAutoTagger(
        string autoTaggerName,
        [FromServices] IEnumerable<AutoTagger> autoTaggers,
        [FromServices] AutoTaggerQueue autoTaggerQueue)
    {
        AutoTagger? autoTagger = autoTaggers.FirstOrDefault(autoTagger => autoTagger.Name == autoTaggerName);
        if (autoTagger == null)
            return DistagsProblemResults.AutoTaggerNotFound(autoTaggerName);

        if (!autoTaggerQueue.TryEnqueue(autoTagger.GetType()))
            return DistagsProblemResults.AutoTaggerAlreadyQueued(autoTaggerName);

        return NoContent();
    }
}
