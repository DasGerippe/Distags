using DataClasses;

namespace AutoTaggerRunner;

public interface IAutoTaggerLevelProvider
{
    Task<List<Level>> GetLevelsToTagAsync(AutoTaggerState state);
}