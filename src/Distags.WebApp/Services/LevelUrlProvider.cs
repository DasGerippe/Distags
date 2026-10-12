using Distags.Api.Contracts.Levels;

namespace Distags.WebApp.Services;

public class LevelUrlProvider
{
    private readonly string _DistagsBaseUrl;

    public LevelUrlProvider(IConfiguration configuration)
    {
        _DistagsBaseUrl = configuration["DistagsWebApp:DistagsApiUri"]?.TrimEnd('/')
                          ?? throw new ArgumentException("Configuration is missing a Distags API Uri.", nameof(configuration));
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
            OfficialLevelResponse officialLevel => $"{_DistagsBaseUrl}/images/levels/official/{officialLevel.Slug}.png",
            WorkshopLevelResponse workshopLevel => workshopLevel.ImageUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unsupported level type."),
        };
    }
}
