using Distags.Contracts;
using Distags.Core.Mapping;

namespace AutoTagging.Abstractions;

public abstract class AutoTagger
{
    public string Name { get; protected set; }

    public abstract TaggingStrategy TaggingStrategy { get; }

    public abstract LevelProjectionIncludes RequiredIncludes { get; }

    protected AutoTagger()
    {
        Name = GetType().Name;
    }

    public abstract Task<TagDelta> CalculateTagDeltaAsync(LevelProjectionDto level, CancellationToken cancellationToken = default);
}