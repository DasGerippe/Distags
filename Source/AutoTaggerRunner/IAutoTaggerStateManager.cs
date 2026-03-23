using AutoTagging.Abstractions;

namespace AutoTaggerRunner;

public interface IAutoTaggerStateManager
{
    Task<AutoTaggerState> LoadAutoTaggerStateAsync(AutoTagger autoTagger);

    Task SaveAutoTaggerStateAsync(AutoTagger autoTagger, AutoTaggerState state);
}