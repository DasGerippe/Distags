using AutoTagging.Abstractions.Progress;
using Distags.Contracts;
using Distags.Core.Queries.AutoTaggerLevels;

namespace AutoTagging.Abstractions.TaggingStrategy;

public class AllLevelsTaggingStrategy : ITaggingStrategy
{
    public AutoTaggerLevelsQuery GetLevelsQuery(IAutoTaggerProgress? progress)
    {
        return new AutoTaggerLevelsQuery();
    }

    public IAutoTaggerProgress? UpdateProgress(IAutoTaggerProgress? progress, LevelDto level)
    {
        return null;
    }
}
