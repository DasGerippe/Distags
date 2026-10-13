using Distags.Core.Entities;

namespace Distags.Core.Management;

public class LevelFactory
{
    private readonly FallbackTagProvider _FallbackTagProvider;

    public LevelFactory(FallbackTagProvider fallbackTagProvider)
    {
        _FallbackTagProvider = fallbackTagProvider;
    }

    public async Task<OfficialLevel> CreateOfficialLevelAsync()
    {
        OfficialLevel officialLevel = new OfficialLevel();
        await InitializeLevelAsync(officialLevel).ConfigureAwait(false);
        return officialLevel;
    }

    public async Task<WorkshopLevel> CreateWorkshopLevelAsync()
    {
        WorkshopLevel workshopLevel = new WorkshopLevel();
        await InitializeLevelAsync(workshopLevel).ConfigureAwait(false);
        return workshopLevel;
    }

    private async Task InitializeLevelAsync(Level level)
    {
        IReadOnlyCollection<Tag> fallbackTags = await _FallbackTagProvider.GetFallbackTagsAsync().ConfigureAwait(false);
        level.Tags.AddRange(fallbackTags);
    }
}
