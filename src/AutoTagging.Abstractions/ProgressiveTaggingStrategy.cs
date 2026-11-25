using AutoTagging.Abstractions.Progress;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Queries.AutoTaggerLevels;
using Microsoft.Extensions.Logging;

namespace AutoTagging.Abstractions;

public class ProgressiveTaggingStrategy<TProgressStrategy, TTagDeltaCalculator> : ITaggingStrategy
    where TProgressStrategy : class, IProgressStrategy
    where TTagDeltaCalculator : class, ITagDeltaCalculator
{
    private readonly string _StateKey;
    private readonly StateStore _StateStore;
    private readonly TagChanger _TagChanger;
    private readonly TProgressStrategy _ProgressStrategy;
    private readonly TTagDeltaCalculator _TagDeltaCalculator;
    private readonly IRequestDispatcher _RequestDispatcher;
    private readonly ILogger<ProgressiveTaggingStrategy<TProgressStrategy, TTagDeltaCalculator>> _Logger;

    public ProgressiveTaggingStrategy(
        string stateKey,
        StateStore stateStore,
        TagChanger tagChanger,
        TProgressStrategy progressStrategy,
        TTagDeltaCalculator tagDeltaCalculator,
        IRequestDispatcher requestDispatcher,
        ILogger<ProgressiveTaggingStrategy<TProgressStrategy, TTagDeltaCalculator>> logger)
    {
        _StateKey = stateKey;
        _StateStore = stateStore;
        _TagChanger = tagChanger;
        _ProgressStrategy = progressStrategy;
        _TagDeltaCalculator = tagDeltaCalculator;
        _RequestDispatcher = requestDispatcher;
        _Logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _Logger.LogDebug("Executing {strategy} with {calculator}...",
            nameof(ProgressiveTaggingStrategy<,>), typeof(TTagDeltaCalculator).Name);

        CursorState state = await _StateStore.GetStateAsync<CursorState>(_StateKey).ConfigureAwait(false);

        AutoTaggerLevelsQuery levelsQuery = _ProgressStrategy.GetLevelsQuery(state.Cursor);
        List<LevelDto> levels = await _RequestDispatcher.DispatchAsync(levelsQuery).ConfigureAwait(false);

        _Logger.LogDebug("Got {levelCount} levels to tag.", levels.Count);

        try
        {
            foreach (LevelDto level in levels)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                _Logger.LogDebug("Calculating tag delta for {levelType} level {levelName}...", level.Type, level.Name);

                TagDelta tagDelta = await _TagDeltaCalculator.CalculateTagDeltaAsync(level, cancellationToken)
                    .ConfigureAwait(false);
                await _TagChanger.ApplyTagDeltaAsync(level, tagDelta).ConfigureAwait(false);

                state.Cursor = _ProgressStrategy.UpdateCursor(state.Cursor, level);
            }
        }
        finally
        {
            await _StateStore.SetStateAsync(_StateKey, state).ConfigureAwait(false);
        }
    }
}
