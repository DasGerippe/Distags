using Distags.Api.Contracts.Levels;

namespace Distags.WebApp.Services;

public class LevelUrlProvider
{
    private readonly string _ApiUrlExternal;

    public LevelUrlProvider(DistagsWebAppSettings webAppSettings)
    {
        _ApiUrlExternal = webAppSettings.DistagsApiUrlExternal.TrimEnd('/');
    }

    public string GetLevelUrlRelative(LevelResponse level)
    {
        return level switch
        {
            OfficialLevelResponse officialLevel => $"level/official/{officialLevel.Slug}",
            WorkshopLevelResponse workshopLevel => $"level/workshop/{workshopLevel.WorkshopId}",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unsupported level type."),
        };
    }

    public string GetLevelImageUrl(LevelResponse level)
    {
        return level switch
        {
            OfficialLevelResponse officialLevel => $"{_ApiUrlExternal}/images/levels/official/{officialLevel.Slug}.png",
            WorkshopLevelResponse workshopLevel => workshopLevel.ImageUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unsupported level type."),
        };
    }
}
