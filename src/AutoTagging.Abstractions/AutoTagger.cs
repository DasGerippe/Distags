using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions;

public abstract class AutoTagger
{
    public abstract Guid Guid { get; }

    public virtual string Name => field ??= GetType().Name;

    public abstract TimeSpan? Interval { get; }

    public abstract ITaggingStrategy TaggingStrategy { get; }

    public virtual object? State { get; set; }

    public virtual Type? GetStateType() => null;

    public abstract AutoTaggerLevelsQuery GetLevelsQuery();

    public abstract Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default);

    public abstract void UpdateState(LevelDto level, TagDelta calculatedTagDelta);
}

public abstract class AutoTagger<TState> : AutoTagger
    where TState : class
{
    public new virtual TState? State
    {
        get => base.State as TState;
        set => base.State = value;
    }

    public sealed override Type GetStateType()
    {
        return typeof(TState);
    }
}
