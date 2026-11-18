using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence.Seeding.Tags;

public class TagSeeder
{
    private readonly SeedTagsAndGroupsProvider _SeedTagsAndGroupsProvider;

    public TagSeeder(SeedTagsAndGroupsProvider seedTagsAndGroupsProvider)
    {
        _SeedTagsAndGroupsProvider = seedTagsAndGroupsProvider;
    }

    public async Task SeedGroupsAndTagsAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        Dictionary<string, DisplayGroup> displayGroups = await SyncDisplayGroupsAsync(dbContext, cancellationToken).ConfigureAwait(false);
        Dictionary<string, LogicGroup> logicGroups = await SyncLogicGroupsAsync(dbContext, cancellationToken).ConfigureAwait(false);
        await SyncTagsAsync(dbContext, displayGroups, logicGroups, cancellationToken).ConfigureAwait(false);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Delete empty groups *after* sync so we don't cascade delete tags that are moved to different groups during sync.
        await DeleteEmptyGroupsAsync(dbContext, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<Dictionary<string, DisplayGroup>> SyncDisplayGroupsAsync(
        DbContext dbContext, CancellationToken cancellationToken = default)
    {
        Dictionary<string, DisplayGroup> displayGroupsDict = await dbContext.Set<DisplayGroup>()
            .ToDictionaryAsync(displayGroup => displayGroup.Name, cancellationToken)
            .ConfigureAwait(false);

        List<SeedDisplayGroup> seedDisplayGroups = _SeedTagsAndGroupsProvider.GetSeedDisplayGroups();
        int position = 0;
        foreach (SeedDisplayGroup seedDisplayGroup in seedDisplayGroups)
        {
            if (!displayGroupsDict.TryGetValue(seedDisplayGroup.Name, out DisplayGroup? displayGroup))
            {
                displayGroup = new DisplayGroup { Name = seedDisplayGroup.Name };
                displayGroupsDict[seedDisplayGroup.Name] = displayGroup;
                dbContext.Add(displayGroup);
            }

            displayGroup.DisplayName = seedDisplayGroup.DisplayName;
            displayGroup.Description = seedDisplayGroup.Description;
            displayGroup.Position = ++position;
        }

        return displayGroupsDict;
    }

    private async Task<Dictionary<string, LogicGroup>> SyncLogicGroupsAsync(
        DbContext dbContext, CancellationToken cancellationToken = default)
    {
        Dictionary<string, LogicGroup> logicGroupsDict = await dbContext.Set<LogicGroup>()
            .ToDictionaryAsync(logicGroup => logicGroup.Name, cancellationToken)
            .ConfigureAwait(false);

        List<SeedLogicGroup> seedLogicGroups = _SeedTagsAndGroupsProvider.GetSeedLogicGroups();
        foreach (SeedLogicGroup seedLogicGroup in seedLogicGroups)
        {
            if (!logicGroupsDict.TryGetValue(seedLogicGroup.Name, out LogicGroup? logicGroup))
            {
                logicGroup = new LogicGroup { Name = seedLogicGroup.Name };
                logicGroupsDict[seedLogicGroup.Name] = logicGroup;
                dbContext.Add(logicGroup);
            }

            logicGroup.AllExclusive = seedLogicGroup.AllExclusive;
        }

        return logicGroupsDict;
    }

    private async Task SyncTagsAsync(
        DbContext dbContext,
        Dictionary<string, DisplayGroup> displayGroups,
        Dictionary<string, LogicGroup> logicGroups,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, Tag> tagsDict = await dbContext.Set<Tag>()
            .ToDictionaryAsync(tag => tag.Name, cancellationToken)
            .ConfigureAwait(false);

        List<SeedTag> seedTags = _SeedTagsAndGroupsProvider.GetSeedTags();
        int position = 0;
        DisplayGroup? lastDisplayGroup = null;
        foreach (SeedTag seedTag in seedTags)
        {
            if (!tagsDict.TryGetValue(seedTag.Name, out Tag? tag))
            {
                tag = new Tag { Name = seedTag.Name };
                dbContext.Add(tag);
            }

            tag.DisplayName = seedTag.DisplayName;
            tag.Description = seedTag.Description;
            tag.DisplayGroup = displayGroups[seedTag.DisplayGroupName];
            tag.LogicGroup = string.IsNullOrEmpty(seedTag.LogicGroupName) ? null : logicGroups[seedTag.LogicGroupName];
            tag.IsExclusive = seedTag.IsExclusive;
            tag.IsFallback = seedTag.IsFallback;
            tag.IsAutoTag = seedTag.IsAutoTag;

            if (tag.DisplayGroup != lastDisplayGroup)
                position = 0;

            tag.Position = ++position;
            lastDisplayGroup = tag.DisplayGroup;
        }
    }

    private async Task DeleteEmptyGroupsAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        List<DisplayGroup> emptyDisplayGroups = await dbContext.Set<DisplayGroup>()
            .Where(displayGroup => !displayGroup.Tags.Any())
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        dbContext.RemoveRange(emptyDisplayGroups);

        List<LogicGroup> emptyLogicGroups = await dbContext.Set<LogicGroup>()
            .Where(logicGroup => !logicGroup.Tags.Any())
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        dbContext.RemoveRange(emptyLogicGroups);
    }
}
