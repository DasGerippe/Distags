using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Mapping;
using Distags.Core.Queries;
using Distags.Core.Queries.LevelsProjection;

namespace AutoTaggerRunner;

public class AutoTaggerLevelProviderFactory : IAutoTaggerLevelProviderFactory
{
    private readonly IQueryHandler<LevelsProjectionQuery, List<LevelProjectionDto>> _LevelsProjectionQueryHandler;

    public AutoTaggerLevelProviderFactory(IQueryHandler<LevelsProjectionQuery, List<LevelProjectionDto>> levelsProjectionQueryHandler)
    {
        _LevelsProjectionQueryHandler = levelsProjectionQueryHandler;
    }

    public IAutoTaggerLevelProvider CreateLevelProvider(TaggingStrategy taggingStrategy, LevelProjectionIncludes requiredIncludes)
    {
        return new AutoTaggerLevelProvider(taggingStrategy, requiredIncludes, _LevelsProjectionQueryHandler);
    }
}