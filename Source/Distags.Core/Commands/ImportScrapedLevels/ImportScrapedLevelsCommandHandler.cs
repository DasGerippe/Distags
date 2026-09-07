using Distags.Core.Entities;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ImportScrapedLevels;

public class ImportScrapedLevelsCommandHandler : ICommandHandler<ImportScrapedLevelsCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly FallbackTagProvider _FallbackTagProvider;

    public ImportScrapedLevelsCommandHandler(DistagsDbContext dbContext, FallbackTagProvider fallbackTagProvider)
    {
        _DbContext = dbContext;
        _FallbackTagProvider = fallbackTagProvider;
    }

    public async Task HandleAsync(ImportScrapedLevelsCommand command)
    {
        IEnumerable<long> scrapedWorkshopIds = command.ScrapedLevels.Select(scrapedLevel => scrapedLevel.WorkshopId);
        Dictionary<long, WorkshopLevel> existingLevels = await _DbContext.Set<WorkshopLevel>()
            .Where(level => scrapedWorkshopIds.Contains(level.WorkshopId))
            .ToDictionaryAsync(level => level.WorkshopId)
            .ConfigureAwait(false);

        foreach (ScrapedLevel scrapedLevel in command.ScrapedLevels)
        {
            if (!existingLevels.TryGetValue(scrapedLevel.WorkshopId, out WorkshopLevel? level))
            {
                level = new WorkshopLevel
                {
                    WorkshopId = scrapedLevel.WorkshopId,
                };
                level.Tags.AddRange(await _FallbackTagProvider.GetFallbackTagsAsync().ConfigureAwait(false));
                _DbContext.Add(level);
            }
            SetLevelProperties(level, scrapedLevel);
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private void SetLevelProperties(WorkshopLevel level, ScrapedLevel scrapedLevel)
    {
        level.Name = scrapedLevel.Name;
        level.FileName = scrapedLevel.FileName;
        level.FileUrl = scrapedLevel.FileUrl;
        level.ImageUrl = scrapedLevel.ImageUrl;
        level.CreatorId = scrapedLevel.CreatorId;
        level.CreatedOn = scrapedLevel.CreatedOn;
        level.UpdatedOn = scrapedLevel.UpdatedOn;
        level.WorkshopTags = scrapedLevel.WorkshopTags;
    }
}
