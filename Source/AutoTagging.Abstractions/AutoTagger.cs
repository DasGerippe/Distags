using Distags.Contracts;
using Distags.Core.Mapping;

namespace AutoTagging.Abstractions;

public abstract class AutoTagger
{
    public virtual string Name => GetType().Name;

    public abstract TaggingStrategy TaggingStrategy { get; }

    public abstract LevelProjectionIncludes RequiredIncludes { get; }

    public abstract Task<TagDelta> CalculateTagDeltaAsync(LevelProjectionDto level, CancellationToken cancellationToken = default);
}