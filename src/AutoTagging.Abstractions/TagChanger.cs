using Distags.Contracts;
using Distags.Core;
using Distags.Core.Commands.ChangeLevelTag;
using Distags.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AutoTagging.Abstractions;

public class TagChanger
{
    private readonly AutoTaggerInfo _AutoTaggerInfo;
    private readonly IServiceScopeFactory _ScopeFactory;
    private readonly ILogger<TagChanger> _Logger;

    public TagChanger(
        AutoTaggerInfo autoTaggerInfo,
        IServiceScopeFactory scopeFactory,
        ILogger<TagChanger> logger)
    {
        _AutoTaggerInfo = autoTaggerInfo;
        _ScopeFactory = scopeFactory;
        _Logger = logger;
    }

    public async Task ApplyTagDeltaAsync(LevelDto level, TagDelta tagDelta)
    {
        if (tagDelta.AddTags.Count == 0 && tagDelta.RemoveTags.Count == 0)
            return;

        _Logger.LogDebug("Applying calculated tag delta: +{tagsToAdd}; -{tagsToRemove}", tagDelta.AddTags, tagDelta.RemoveTags);

        AsyncServiceScope scope = _ScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredScope = scope.ConfigureAwait(false);

        IRequestDispatcher requestDispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();

        foreach (string tagToAdd in tagDelta.AddTags)
        {
            await ApplyTagChangeAsync(requestDispatcher, level, tagToAdd, TagChangeAction.Add).ConfigureAwait(false);
        }

        foreach (string tagToRemove in tagDelta.RemoveTags)
        {
            await ApplyTagChangeAsync(requestDispatcher, level, tagToRemove, TagChangeAction.Remove).ConfigureAwait(false);
        }
    }

    private Task ApplyTagChangeAsync(IRequestDispatcher requestDispatcher, LevelDto level, string tagName, TagChangeAction changeAction)
    {
        ChangeLevelTagCommand command = level switch
        {
            OfficialLevelDto officialLevel => new ChangeOfficialLevelTagCommand { LevelSlug = officialLevel.Slug },
            WorkshopLevelDto workshopLevel => new ChangeWorkshopLevelTagCommand { LevelWorkshopId = workshopLevel.WorkshopId },
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };

        command.AutoTaggerGuid = _AutoTaggerInfo.Guid;
        command.TagName = tagName;
        command.Action = changeAction;

        return TryChangeLevelTagAsync(requestDispatcher, command);
    }

    public async Task TryChangeLevelTagAsync(ChangeLevelTagCommand changeLevelTagCommand)
    {
        AsyncServiceScope scope = _ScopeFactory.CreateAsyncScope();
        await using ConfiguredAsyncDisposable configuredScope = scope.ConfigureAwait(false);

        IRequestDispatcher requestDispatcher = scope.ServiceProvider.GetRequiredService<IRequestDispatcher>();

        await TryChangeLevelTagAsync(requestDispatcher, changeLevelTagCommand).ConfigureAwait(false);
    }

    private async Task TryChangeLevelTagAsync(IRequestDispatcher requestDispatcher, ChangeLevelTagCommand changeLevelTagCommand)
    {
        try
        {
            await requestDispatcher.DispatchAsync(changeLevelTagCommand).ConfigureAwait(false);
        }
        catch (UnknownLevelException)
        {
            _Logger.LogWarning("Could not tag unknown level {level}.", changeLevelTagCommand switch
            {
                ChangeOfficialLevelTagCommand officialCommand => officialCommand.LevelSlug,
                ChangeWorkshopLevelTagCommand workshopCommand => workshopCommand.LevelWorkshopId,
                _ => string.Empty,
            });
        }
        catch (UnknownTagException)
        {
            _Logger.LogWarning("Could not change unknown tag '{tagName}'.", changeLevelTagCommand.TagName);
        }
        catch (LevelTagExistsException)
        {
            _Logger.LogWarning("Could not add tag '{tagName}' to level because it was already present.", changeLevelTagCommand.TagName);
        }
        catch (LevelTagNotFoundException)
        {
            _Logger.LogWarning("Could not remove tag '{tagName}' from level because it wasn't present.", changeLevelTagCommand.TagName);
        }
    }
}
