using DataClasses;

namespace AutoTagging.Abstractions;

public abstract class AutoTagger
{
    public virtual string Name => GetType().Name;

    public abstract TaggingStrategy TaggingStrategy { get; }

    public abstract Task<TagDelta> CalculateTagDeltaAsync(Level level, CancellationToken cancellationToken = default);
}