using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core.Filtering.Levels;
using Distags.Core.Mapping;
using Distags.Core.Queries;
using Distags.Core.Queries.LevelsProjection;

namespace AutoTaggerRunner;

public class AutoTaggerLevelProvider : IAutoTaggerLevelProvider
{
    private readonly TaggingStrategy _TaggingStrategy;
    private readonly LevelProjectionIncludes _RequiredIncludes;

    private readonly IQueryHandler<LevelsProjectionQuery, List<LevelProjectionDto>> _LevelsProjectionQueryHandler;

    public AutoTaggerLevelProvider(
        TaggingStrategy taggingStrategy,
        LevelProjectionIncludes requiredIncludes,
        IQueryHandler<LevelsProjectionQuery, List<LevelProjectionDto>> levelsProjectionQueryHandler)
    {
        _TaggingStrategy = taggingStrategy;
        _RequiredIncludes = requiredIncludes;
        _LevelsProjectionQueryHandler = levelsProjectionQueryHandler;
    }

    public async Task<List<LevelProjectionDto>> GetLevelsToTagAsync(AutoTaggerState state)
    {
        LevelFilterParameters filterParameters = new LevelFilterParameters();
        switch (_TaggingStrategy)
        {
            case TaggingStrategy.NewLevels:
                filterParameters.CreatedAfter = state.LastAutoTagging;
                break;
            case TaggingStrategy.NewAndUpdatedLevels:
                filterParameters.UpdatedAfter = state.LastAutoTagging;
                break;
        }

        LevelsProjectionQuery query = new LevelsProjectionQuery
        {
            FilterParameters = filterParameters,
            Includes = _RequiredIncludes,
        };
        List<LevelProjectionDto> levels = await _LevelsProjectionQueryHandler.HandleAsync(query);
        return levels;
    }
}