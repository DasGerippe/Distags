using AutoTagging.Abstractions;
using DataClasses;

namespace AutoTaggerRunner;

public interface IAutoTaggerStateUpdater
{
    void UpdateAutoTaggerState(AutoTaggerState state, Level level, TaggingStrategy taggingStrategy);
}