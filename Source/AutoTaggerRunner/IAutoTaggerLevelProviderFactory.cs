using AutoTagging.Abstractions;

namespace AutoTaggerRunner;

public interface IAutoTaggerLevelProviderFactory
{
    IAutoTaggerLevelProvider CreateLevelProvider(TaggingStrategy taggingStrategy);
}