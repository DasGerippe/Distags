using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ImportLevels;

public class ImportLevelsCommandHandler : ICommandHandler<ImportLevelsCommand>
{
    private readonly DbContext _DbContext;

    public ImportLevelsCommandHandler(DbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(ImportLevelsCommand command)
    {
        IEnumerable<long> scrapedLevelIds = command.ScrapedLevels.Select(scrapedLevel => scrapedLevel.Id);
        Dictionary<long, Level> existingLevels = await _DbContext.Set<Level>()
            .Where(level => scrapedLevelIds.Contains(level.Id))
            .ToDictionaryAsync(level => level.Id)
            .ConfigureAwait(false);

        foreach (ScrapedLevel scrapedLevel in command.ScrapedLevels)
        {
            if (!existingLevels.TryGetValue(scrapedLevel.Id, out Level? level))
            {
                level = new Level { Id = scrapedLevel.Id };
                _DbContext.Add(level);
            }
            SetLevelProperties(level, scrapedLevel);
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private void SetLevelProperties(Level level, ScrapedLevel scrapedLevel)
    {
        level.Name = scrapedLevel.Name;
        level.FileName = scrapedLevel.FileName;
        level.FileUrl = scrapedLevel.FileUrl;
        level.IconUrl = scrapedLevel.ImageUrl;
        level.CreatorId = scrapedLevel.CreatorId;
        level.CreatedOn = scrapedLevel.CreatedOn;
        level.UpdatedOn = scrapedLevel.UpdatedOn;
        level.WorkshopTags = scrapedLevel.WorkshopTags.ToArray();
    }
}