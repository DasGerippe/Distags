using Distags.Core.Entities;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ImportWorkshopLevels;

public class ImportWorkshopLevelsCommandHandler : ICommandHandler<ImportWorkshopLevelsCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly LevelFactory _LevelFactory;

    public ImportWorkshopLevelsCommandHandler(DistagsDbContext dbContext, LevelFactory levelFactory)
    {
        _DbContext = dbContext;
        _LevelFactory = levelFactory;
    }

    public async Task HandleAsync(ImportWorkshopLevelsCommand command)
    {
        IEnumerable<long> scrapedWorkshopIds = command.ScrapedWorkshopLevels.Select(scrapedWorkshopLevel => scrapedWorkshopLevel.WorkshopId);
        Dictionary<long, WorkshopLevel> existingLevels = await _DbContext.Set<WorkshopLevel>()
            .Where(workshopLevel => scrapedWorkshopIds.Contains(workshopLevel.WorkshopId))
            .ToDictionaryAsync(workshopLevel => workshopLevel.WorkshopId)
            .ConfigureAwait(false);

        foreach (ScrapedWorkshopLevel scrapedWorkshopLevel in command.ScrapedWorkshopLevels)
        {
            if (!existingLevels.TryGetValue(scrapedWorkshopLevel.WorkshopId, out WorkshopLevel? workshopLevel))
            {
                workshopLevel = await _LevelFactory.CreateWorkshopLevelAsync().ConfigureAwait(false);
                workshopLevel.WorkshopId = scrapedWorkshopLevel.WorkshopId;
                _DbContext.Add(workshopLevel);
            }
            SetLevelProperties(workshopLevel, scrapedWorkshopLevel);
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private void SetLevelProperties(WorkshopLevel workshopLevel, ScrapedWorkshopLevel scrapedWorkshopLevel)
    {
        workshopLevel.Name = scrapedWorkshopLevel.Name;
        workshopLevel.FileName = scrapedWorkshopLevel.FileName;
        workshopLevel.FileUrl = scrapedWorkshopLevel.FileUrl;
        workshopLevel.ImageUrl = scrapedWorkshopLevel.ImageUrl;
        workshopLevel.CreatorId = scrapedWorkshopLevel.CreatorId;
        workshopLevel.CreatedOn = scrapedWorkshopLevel.CreatedOn;
        workshopLevel.UpdatedOn = scrapedWorkshopLevel.UpdatedOn;
        workshopLevel.WorkshopTags = scrapedWorkshopLevel.WorkshopTags;
    }
}
