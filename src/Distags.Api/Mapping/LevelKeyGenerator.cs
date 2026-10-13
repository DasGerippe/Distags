using Distags.Contracts;

namespace Distags.Api.Mapping;

public static class LevelKeyGenerator
{
    public static string GetKey(LevelDto level)
    {
        return level switch
        {
            OfficialLevelDto officialLevel => $"official/{officialLevel.Slug}",
            WorkshopLevelDto workshopLevel => $"workshop/{workshopLevel.WorkshopId}",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level.GetType().FullName, "Unsupported level type."),
        };
    }
}
