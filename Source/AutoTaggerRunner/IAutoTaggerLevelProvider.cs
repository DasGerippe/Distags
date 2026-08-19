using Distags.Contracts;

namespace AutoTaggerRunner;

public interface IAutoTaggerLevelProvider
{
    Task<List<LevelProjectionDto>> GetLevelsToTagAsync(AutoTaggerState state);
}
