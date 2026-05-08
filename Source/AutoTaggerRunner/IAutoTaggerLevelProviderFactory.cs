using AutoTagging.Abstractions;
using Distags.Core.Mapping;

namespace AutoTaggerRunner;

public interface IAutoTaggerLevelProviderFactory
{
    IAutoTaggerLevelProvider CreateLevelProvider(TaggingStrategy taggingStrategy, LevelProjectionIncludes requiredIncludes);
}