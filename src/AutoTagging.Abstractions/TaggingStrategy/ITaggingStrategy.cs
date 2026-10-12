using AutoTagging.Abstractions.Progress;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions.TaggingStrategy;

public interface ITaggingStrategy
{
    AutoTaggerLevelsQuery GetLevelsQuery(IAutoTaggerProgress? progress);

    IAutoTaggerProgress? UpdateProgress(IAutoTaggerProgress? progress, LevelDto level);
}

public interface ITaggingStrategy<TProgress> : ITaggingStrategy
    where TProgress : IAutoTaggerProgress
{
    AutoTaggerLevelsQuery GetLevelsQuery(TProgress? progress);

    TProgress? UpdateProgress(TProgress? progress, LevelDto level);

    AutoTaggerLevelsQuery ITaggingStrategy.GetLevelsQuery(IAutoTaggerProgress? progress)
    {
        return GetLevelsQuery((TProgress?)progress);
    }

    IAutoTaggerProgress? ITaggingStrategy.UpdateProgress(IAutoTaggerProgress? progress, LevelDto level)
    {
        return UpdateProgress((TProgress?)progress, level);
    }
}
