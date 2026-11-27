using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggerLevels;
using Microsoft.Extensions.Logging;

namespace AutoTagging.Length;

public class LengthTaggingStrategy : ITaggingStrategy
{
    private const string StateKey = "";

    private readonly SeekrDbReader _SeekrDbReader;
    private readonly StateStore _StateStore;
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly TagChanger _TagChanger;
    private readonly ILogger<LengthTaggingStrategy> _Logger;

    public LengthTaggingStrategy(
        SeekrDbReader seekrDbReader,
        StateStore stateStore,
        IRequestDispatcher requestDispatcher,
        TagChanger tagChanger,
        ILogger<LengthTaggingStrategy> logger)
    {
        _SeekrDbReader = seekrDbReader;
        _StateStore = stateStore;
        _RequestDispatcher = requestDispatcher;
        _TagChanger = tagChanger;
        _Logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Executing {strategy}...", nameof(LengthTaggingStrategy));

        IReadOnlyCollection<LevelStats> levelStats = await _SeekrDbReader.QueryLevelStatsAsync(cancellationToken).ConfigureAwait(false);
        LevelStatsLookup levelStatsLookup = new LevelStatsLookup(levelStats);

        _Logger.LogDebug("Queried {count} level stats...", levelStats.Count);

        LengthTaggingState state = await _StateStore.GetStateAsync<LengthTaggingState>(StateKey).ConfigureAwait(false);

        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(new AutoTaggerLevelsQuery()).ConfigureAwait(false);

        _Logger.LogDebug("Got {count} level to tag.", levels.Count);

        try
        {
            foreach (LevelDto level in levels)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                LevelStats? stats = levelStatsLookup.GetLevelStats(level);
                if (stats == null)
                    continue;

                TagDelta tagDelta = CalculateTagDelta(level, stats, state);
                await _TagChanger.ApplyTagDeltaAsync(level, tagDelta).ConfigureAwait(false);
            }
        }
        finally
        {
            await _StateStore.SetStateAsync(StateKey, state).ConfigureAwait(false);
        }    
    }

    private TagDelta CalculateTagDelta(LevelDto level, LevelStats stats, LengthTaggingState state)
    {
        TagDelta tagDelta = new TagDelta();

        string wrLengthTagName = LengthTagResolver.GetWrLengthTag(stats.WorldRecordTime);
        if (!level.Tags.Contains(wrLengthTagName))
            tagDelta.AddTags.Add(wrLengthTagName);

        string? lengthTagName = GetLengthTagName(level, stats, state);
        if (lengthTagName != null && !level.Tags.Contains(lengthTagName))
            tagDelta.AddTags.Add(lengthTagName);

        return tagDelta;
    }

    private string? GetLengthTagName(LevelDto level, LevelStats stats, LengthTaggingState state)
    {
        // Less than 10 entries is too noisy.
        if (stats.EntryCount < 10)
            return null;

        LengthTagInterval lengthTagInterval = LengthTagResolver.GetLengthTagInterval(stats.MedianTime);

        bool confident =
            // 30 or more entries should be pretty stable.
            stats.EntryCount >= 30 ||
            // A confidence interval with at most 80% the width of the length tag's range probably means stable times.
            stats.CiHighTime - stats.CiLowTime <= 0.8 * lengthTagInterval.Length;

        if (confident)
        {
            state.AppliedUncertainLevelTags.Remove(level.Id);
        }
        else
        {
            string? previousUnconfidentLengthTag = state.AppliedUncertainLevelTags.GetValueOrDefault(level.Id);
            if ((previousUnconfidentLengthTag == null && !level.Tags.Contains("length-unspecified")) ||
                (previousUnconfidentLengthTag != null && !level.Tags.Contains(previousUnconfidentLengthTag)))
                // Length has been set by a user. Do not override since we aren't confident.
                return null;

            state.AppliedUncertainLevelTags[level.Id] = lengthTagInterval.TagName;
        }

        return lengthTagInterval.TagName;
    }
}
