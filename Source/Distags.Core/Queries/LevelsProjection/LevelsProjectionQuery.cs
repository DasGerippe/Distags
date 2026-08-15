using Distags.Contracts;
using Distags.Core.Filtering.Levels;
using Distags.Core.Mapping;

namespace Distags.Core.Queries.LevelsProjection;

public class LevelsProjectionQuery : IQuery<List<LevelProjectionDto>>
{
    public LevelFilterParameters FilterParameters { get; set; } = new LevelFilterParameters();

    public LevelProjectionIncludes Includes { get; set; }
}
