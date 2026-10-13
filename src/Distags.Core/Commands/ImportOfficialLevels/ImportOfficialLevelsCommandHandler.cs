using Distags.Core.Entities;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace Distags.Core.Commands.ImportOfficialLevels;

public class ImportOfficialLevelsCommandHandler : ICommandHandler<ImportOfficialLevelsCommand>
{
    private readonly DistagsDbContext _DbContext;
    private readonly LevelFactory _LevelFactory;
    private readonly ISlugHelper _SlugHelper;

    public ImportOfficialLevelsCommandHandler(DistagsDbContext dbContext, LevelFactory levelFactory, ISlugHelper slugHelper)
    {
        _DbContext = dbContext;
        _LevelFactory = levelFactory;
        _SlugHelper = slugHelper;
    }

    public async Task HandleAsync(ImportOfficialLevelsCommand command)
    {
        Dictionary<string, OfficialLevel> dbOfficialLevels = await _DbContext.Set<OfficialLevel>()
            .ToDictionaryAsync(officialLevel => officialLevel.Slug)
            .ConfigureAwait(false);

        foreach (ScrapedOfficialLevel scrapedOfficialLevel in command.ScrapedOfficialLevels)
        {
            string levelSlug = _SlugHelper.GenerateSlug(scrapedOfficialLevel.Name);

            if (!dbOfficialLevels.TryGetValue(levelSlug, out OfficialLevel? officialLevel))
            {
                officialLevel = await _LevelFactory.CreateOfficialLevelAsync().ConfigureAwait(false);
                officialLevel.Slug = levelSlug;
                _DbContext.Add(officialLevel);
            }
            SetLevelProperties(officialLevel, scrapedOfficialLevel);
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private void SetLevelProperties(OfficialLevel officialLevel, ScrapedOfficialLevel scrapedOfficialLevel)
    {
        officialLevel.Name = scrapedOfficialLevel.Name;
        officialLevel.CreatorName = scrapedOfficialLevel.CreatorName;
        officialLevel.GameModes = scrapedOfficialLevel.GameModes;
    }
}
