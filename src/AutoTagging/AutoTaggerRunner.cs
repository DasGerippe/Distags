using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Commands.CreateOrUpdateAutoTagger;
using Distags.Core.Exceptions;
using Distags.Core.Queries.AutoTaggerByGuid;
using Distags.Core.Queries.AutoTaggerLevels;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AutoTagging;

public class AutoTaggerRunner
{
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly ILogger<AutoTaggerRunner> _Logger;

    public AutoTaggerRunner(IRequestDispatcher requestDispatcher, ILogger<AutoTaggerRunner> logger)
    {
        _RequestDispatcher = requestDispatcher;
        _Logger = logger;
    }

    public async Task RunAsync(AutoTagger autoTagger, CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Running {autoTagger}...", autoTagger.Name);

        AutoTaggerByGuidQuery query = new AutoTaggerByGuidQuery { AutoTaggerGuid = autoTagger.Guid };
        AutoTaggerDto? autoTaggerDto = await _RequestDispatcher.DispatchAsync(query);

        if (autoTaggerDto == null || autoTaggerDto.Name != autoTagger.Name)
            await CreateOrUpdateAutoTagger(autoTagger);

        LoadAutoTaggerState(autoTagger, autoTaggerDto);

        List<LevelDto> levels = await GetLevelsForAutoTagger(autoTagger);
        _Logger.LogDebug("Got {levelCount} levels to tag.", levels.Count);

        try
        {
            foreach (LevelDto level in levels)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                _Logger.LogDebug("Calculating tag delta for {levelType} level {levelName} with {autoTagger}...",
                    level.Type, level.Name, autoTagger.Name);

                TagDelta tagDelta = await autoTagger.CalculateTagDeltaAsync(level, cancellationToken);

                if (tagDelta.AddTags.Count > 0 || tagDelta.RemoveTags.Count > 0)
                {
                    _Logger.LogDebug("Calculated tag delta: +{tagsToAdd}; -{tagsToRemove}", tagDelta.AddTags, tagDelta.RemoveTags);
                    await ApplyTagDeltaAsync(autoTagger, level, tagDelta);
                }

                autoTagger.UpdateState(level, tagDelta);
            }
        }
        finally
        {
            await UpdateAutoTaggerStateAndLastRun(autoTagger);
        }
    }

    private Task CreateOrUpdateAutoTagger(AutoTagger autoTagger)
    {
        CreateOrUpdateAutoTaggerCommand createOrUpdateCommand = new CreateOrUpdateAutoTaggerCommand
        {
            Guid = autoTagger.Guid,
            Name = autoTagger.Name,
        };
        return _RequestDispatcher.DispatchAsync(createOrUpdateCommand);
    }

    private void LoadAutoTaggerState(AutoTagger autoTagger, AutoTaggerDto? autoTaggerDto)
    {
        if (autoTaggerDto?.State == null)
            return;

        Type? stateType = autoTagger.GetStateType();
        if (stateType == null)
            return;

        try
        {
            autoTagger.State = JsonSerializer.Deserialize(autoTaggerDto.State, stateType);
        }
        catch
        {
            _Logger.LogWarning(
                "Could not deserialize state of AutoTagger {autoTagger}. Continuing with uninitialized state.", autoTagger.Guid);
        }
    }

    private async Task UpdateAutoTaggerStateAndLastRun(AutoTagger autoTagger)
    {
        CreateOrUpdateAutoTaggerCommand command = new CreateOrUpdateAutoTaggerCommand
        {
            Guid = autoTagger.Guid,
            LastRunOn = DateTimeOffset.UtcNow,
            State = SerializeAutoTaggerState(autoTagger),
        };
        await _RequestDispatcher.DispatchAsync(command);
    }

    private string? SerializeAutoTaggerState(AutoTagger autoTagger)
    {
        if (autoTagger.State == null)
            return null;

        Type? stateType = autoTagger.GetStateType();
        return stateType == null
            ? null :
            JsonSerializer.Serialize(autoTagger.State, stateType);
    }

    private async Task<List<LevelDto>> GetLevelsForAutoTagger(AutoTagger autoTagger)
    {
        AutoTaggerLevelsQuery levelsQuery = autoTagger.GetLevelsQuery();
        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(levelsQuery);
        return levels;
    }

    private async Task ApplyTagDeltaAsync(AutoTagger autoTagger, LevelDto level, TagDelta tagDelta)
    {
        _Logger.LogDebug("Applying tag delta...");

        foreach (string tagToAdd in tagDelta.AddTags)
        {
            await ApplyTagChangeAsync(autoTagger, level, tagToAdd, TagChangeAction.Add);
        }

        foreach (string tagToRemove in tagDelta.RemoveTags)
        {
            await ApplyTagChangeAsync(autoTagger, level, tagToRemove, TagChangeAction.Remove);
        }
    }

    private async Task ApplyTagChangeAsync(AutoTagger autoTagger, LevelDto level, string tagName, TagChangeAction changeAction)
    {
        try
        {
            ChangeLevelTagCommand command = level switch
            {
                OfficialLevelDto officialLevel => new ChangeOfficialLevelTagCommand { LevelSlug = officialLevel.Slug },
                WorkshopLevelDto workshopLevel => new ChangeWorkshopLevelTagCommand { LevelWorkshopId = workshopLevel.WorkshopId },
                _ => throw new ArgumentOutOfRangeException(nameof(level)),
            };

            command.AutoTaggerGuid = autoTagger.Guid;
            command.TagName = tagName;
            command.Action = changeAction;

            await _RequestDispatcher.DispatchAsync(command);
        }
        catch (UnknownTagException)
        {
            _Logger.LogWarning("Could not change unknown tag '{tagName}'.", tagName);
        }
        catch (LevelTagExistsException)
        {
            _Logger.LogWarning("Could not add tag '{tagName}' to level because it was already present.", tagName);
        }
        catch (LevelTagNotFoundException)
        {
            _Logger.LogWarning("Could not remove tag '{tagName}' from level because it wasn't present.", tagName);
        }
    }
}
