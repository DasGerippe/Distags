using DataClasses;
using DataStoring;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Hubs;

public class LevelTagBroadcaster : ILevelTagBroadcaster
{
    private readonly IHubContext<LevelHub, ILevelHubClient> _LevelHub;
    private readonly LevelTagDbContext _DbContext;

    public LevelTagBroadcaster(
        IHubContext<LevelHub, ILevelHubClient> levelHub,
        LevelTagDbContext dbContext)
    {
        _LevelHub = levelHub;
        _DbContext = dbContext;
    }

    public async Task BroadcastLevelTagChangesAsync(IEnumerable<LevelTag> addedLevelTags, IEnumerable<LevelTag> removedLevelTags)
    {
        ICollection<LevelTag> addedCollection = addedLevelTags as ICollection<LevelTag> ?? addedLevelTags.ToList();
        ICollection<LevelTag> removedCollection = removedLevelTags as ICollection<LevelTag> ?? removedLevelTags.ToList();

        IEnumerable<long> tagIds = Enumerable.Union(
            addedCollection.Select(levelTag => levelTag.TagId),
            removedCollection.Select(levelTag => levelTag.TagId));

        // Load all relevant tags into context at once, so we can access their names.
        await _DbContext.Set<Tag>().Where(tag => tagIds.Contains(tag.Id)).LoadAsync();

        foreach (LevelTag levelTag in removedCollection)
        {
            // EF Core won't populate LevelTag.Tag of untracked LevelTags, so we have to find the Tag ourselves.
            // This shouldn't cause a database query because we loaded all relevant tags earlier.
            Tag removedTag = (await _DbContext.FindAsync<Tag>(levelTag.TagId))!;
            await GetLevelGroup(levelTag.LevelId).TagRemoved(removedTag.Name);
        }

        foreach (LevelTag levelTag in addedCollection)
        {
            // EF Core populated LevelTag.Tag when we loaded all relevant tags earlier.
            await GetLevelGroup(levelTag.LevelId).TagAdded(levelTag.Tag.Name);
        }
    }

    private ILevelHubClient GetLevelGroup(long levelId)
    {
        return _LevelHub.Clients.Group(levelId.ToString());
    }
}