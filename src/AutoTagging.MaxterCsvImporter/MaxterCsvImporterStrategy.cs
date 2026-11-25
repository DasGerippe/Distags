using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggerLevels;
using Microsoft.Extensions.Logging;

namespace AutoTagging.MaxterCsvImporter;

public class MaxterCsvImporterStrategy : ITaggingStrategy
{
    private const string StateKey = "";

    private readonly AutoTaggerInfo _AutoTaggerInfo;
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly StateStore _StateStore;
    private readonly ImportTagDeltaCalculator _TagDeltaCalculator;
    private readonly TagChanger _TagChanger;
    private readonly ILogger<MaxterCsvImporterStrategy> _Logger;

    public MaxterCsvImporterStrategy(
        AutoTaggerInfo autoTaggerInfo,
        IRequestDispatcher requestDispatcher,
        StateStore stateStore,
        ImportTagDeltaCalculator tagDeltaCalculator,
        TagChanger tagChanger,
        ILogger<MaxterCsvImporterStrategy> logger)
    {
        _AutoTaggerInfo = autoTaggerInfo;
        _RequestDispatcher = requestDispatcher;
        _StateStore = stateStore;
        _TagDeltaCalculator = tagDeltaCalculator;
        _TagChanger = tagChanger;
        _Logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        MaxterCsvImporterState state = await _StateStore.GetStateAsync<MaxterCsvImporterState>(StateKey).ConfigureAwait(false);

        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(new AutoTaggerLevelsQuery()).ConfigureAwait(false);

        foreach (LevelDto level in levels)
        {
            _Logger.LogDebug("Calculating tag delta for {levelType} level {levelName} with {autoTagger}...",
                level.Type, level.Name, _AutoTaggerInfo.Name);

            TagDelta tagDelta = _TagDeltaCalculator.CalculateTagDelta(level, state.AppliedTagsLog);
            await _TagChanger.ApplyTagDeltaAsync(level, tagDelta).ConfigureAwait(false);

            UpdateAppliedTagsLog(state.AppliedTagsLog, level, tagDelta);
        }

        await _StateStore.SetStateAsync(StateKey, state).ConfigureAwait(false);
    }

    private void UpdateAppliedTagsLog(Dictionary<long, List<string>> appliedTagsLog, LevelDto level, TagDelta tagDelta)
    {
        long levelId = level.Id;
        List<string> oldAppliedLevelTags = appliedTagsLog.GetValueOrDefault(levelId, []);
        List<string> newAppliedLevelTags = oldAppliedLevelTags
            .Except(tagDelta.RemoveTags)
            .Concat(tagDelta.AddTags)
            .ToList();

        if (newAppliedLevelTags.Count == 0)
            appliedTagsLog.Remove(levelId);
        else
            appliedTagsLog[levelId] = newAppliedLevelTags;
    }
}
