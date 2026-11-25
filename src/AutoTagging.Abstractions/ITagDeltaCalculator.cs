using Distags.Contracts;

namespace AutoTagging.Abstractions;

public interface ITagDeltaCalculator
{
    Task<TagDelta> CalculateTagDeltaAsync(LevelDto level, CancellationToken cancellationToken = default);
}
