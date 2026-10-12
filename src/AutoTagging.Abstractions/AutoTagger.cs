using AutoTagging.Abstractions.TaggingStrategy;
using Distags.Contracts;

namespace AutoTagging.Abstractions;

public abstract class AutoTagger
{
    public virtual string Name => field ??= GetType().Name;

    public abstract ITaggingStrategy TaggingStrategy { get; }

    public abstract Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default);
}
