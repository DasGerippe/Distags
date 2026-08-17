using AutoTagging.Abstractions;
using Distags.Contracts;
using Distags.Core;
using Distags.Core.Filtering.Levels;
using Distags.Core.Mapping;
using Distags.Core.Queries.LevelsProjection;

namespace AutoTaggerRunner;

public class AutoTaggerLevelProvider : IAutoTaggerLevelProvider
{
    private readonly TaggingStrategy _TaggingStrategy;
    private readonly LevelProjectionIncludes _RequiredIncludes;

    private readonly IRequestDispatcher _RequestDispatcher;

    public AutoTaggerLevelProvider(
        TaggingStrategy taggingStrategy,
        LevelProjectionIncludes requiredIncludes,
        IRequestDispatcher requestDispatcher)
    {
        _TaggingStrategy = taggingStrategy;
        _RequiredIncludes = requiredIncludes;
        _RequestDispatcher = requestDispatcher;
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
            case TaggingStrategy.AllLevels:
                break;
        }

        LevelsProjectionQuery query = new LevelsProjectionQuery
        {
            FilterParameters = filterParameters,
            Includes = _RequiredIncludes,
        };
        List<LevelProjectionDto> levels = await _RequestDispatcher.DispatchAsync(query);
        return levels;
    }
}