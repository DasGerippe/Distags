using Distags.Api.Contracts.Levels;

namespace Distags.WebApp.Services;

public class LevelCreatorNameProvider
{
    public string GetCreatorName(LevelResponse level)
    {
        return level switch
        {
            OfficialLevelResponse officialLevel => officialLevel.CreatorName,
            WorkshopLevelResponse workshopLevel => workshopLevel.CreatorName ?? Resource.Creator_UnknownUserName,
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }
}
