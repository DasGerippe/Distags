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
        bool shouldSeedGroupsAndTags =
            !await dbContext.Set<DisplayGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await dbContext.Set<LogicGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await dbContext.Set<Tag>().AnyAsync(cancellationToken).ConfigureAwait(false);

        if (!shouldSeedGroupsAndTags)
            return;

        Dictionary<string, DisplayGroup> displayGroups = CreateDisplayGroupsDictionary();
        Dictionary<string, LogicGroup> logicGroups = CreateLogicGroupDictionary();
        List<Tag> tags = CreateTags(displayGroups, logicGroups);

        dbContext.AddRange(displayGroups.Values);
        dbContext.AddRange(logicGroups.Values);
        dbContext.AddRange(tags);

        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private Dictionary<string, DisplayGroup> CreateDisplayGroupsDictionary()
    {
        int position = 0;

        return _SeedTagsAndGroupsProvider.GetSeedDisplayGroups()
            .Select(seedDisplayGroup => new DisplayGroup
            {
                Name = seedDisplayGroup.Name,
                DisplayName = seedDisplayGroup.DisplayName,
                Description = seedDisplayGroup.Description,
                Position = ++position,
            })
            .ToDictionary(displayGroup => displayGroup.Name);
    }

    private Dictionary<string, LogicGroup> CreateLogicGroupDictionary()
    {
        return _SeedTagsAndGroupsProvider.GetSeedLogicGroups()
            .Select(seedLogicGroup => new LogicGroup()
            {
                Name = seedLogicGroup.Name,
                AllExclusive = seedLogicGroup.AllExclusive,
            })
            .ToDictionary(logicGroup => logicGroup.Name);
    }

    private List<Tag> CreateTags(Dictionary<string, DisplayGroup> displayGroups, Dictionary<string, LogicGroup> logicGroups)
    {
        List<Tag> tags = _SeedTagsAndGroupsProvider.GetSeedTags()
            .Select(seedTag => new Tag
            {
                Name = seedTag.Name,
                DisplayName = seedTag.DisplayName,
                DisplayGroup = displayGroups[seedTag.DisplayGroupName],
                LogicGroup = string.IsNullOrEmpty(seedTag.LogicGroupName) ? null : logicGroups[seedTag.LogicGroupName],
                IsExclusive = seedTag.IsExclusive,
                IsFallback = seedTag.IsFallback,
                IsAutoTag = seedTag.IsAutoTag,
                Description = seedTag.Description,
            })
            .ToList();

        DisplayGroup? lastDisplayGroup = null;
        int position = 0;
        foreach (Tag tag in tags)
        {
            if (tag.DisplayGroup != lastDisplayGroup)
                position = 0;

            tag.Position = ++position;
            lastDisplayGroup = tag.DisplayGroup;
        }

        return tags;
    }
}
