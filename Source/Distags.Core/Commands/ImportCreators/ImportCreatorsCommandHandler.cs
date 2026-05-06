using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Commands.ImportCreators;

public class ImportCreatorsCommandHandler : ICommandHandler<ImportCreatorsCommand>
{
    private readonly DistagsDbContext _DbContext;

    public ImportCreatorsCommandHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task HandleAsync(ImportCreatorsCommand command)
    {
        IEnumerable<long> scrapedCreatorIds = command.ScrapedCreators.Select(scrapedCreator => scrapedCreator.Id);
        Dictionary<long, Creator> existingCreators = await _DbContext.Set<Creator>()
            .Where(creator => scrapedCreatorIds.Contains(creator.Id))
            .ToDictionaryAsync(creator => creator.Id)
            .ConfigureAwait(false);

        foreach (ScrapedCreator scrapedCreator in command.ScrapedCreators)
        {
            if (!existingCreators.TryGetValue(scrapedCreator.Id, out Creator? creator))
            {
                creator = new Creator { Id = scrapedCreator.Id };
                _DbContext.Add(creator);
            }
            SetCreatorProperties(creator, scrapedCreator);
        }

        await _DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private void SetCreatorProperties(Creator creator, ScrapedCreator scrapedCreator)
    {
        creator.Name = scrapedCreator.Name;
        creator.AvatarHash = scrapedCreator.AvatarHash;
        creator.CachedOn = DateTimeOffset.UtcNow;
    }
}