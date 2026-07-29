using Distags.Core.Entities;
using Distags.Core.Management;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence.Seeding;

internal class DataSeeder
{
    private readonly DefaultTagProvider _DefaultTagProvider;
    private readonly FallbackTagProvider _FallbackTagProvider;

    public DataSeeder(DefaultTagProvider defaultTagProvider, FallbackTagProvider fallbackTagProvider)
    {
        _DefaultTagProvider = defaultTagProvider;
        _FallbackTagProvider = fallbackTagProvider;
    }

    public void Seed(DbContext context, bool dataStoreChanged)
    {
        SeedAsync(context, dataStoreChanged, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SeedAsync(DbContext context, bool dataStoreChanged, CancellationToken cancellationToken)
    {
        await SeedDefaultGroupsAndTags(context, cancellationToken).ConfigureAwait(false);
        await SeedLevelFallbackTags(context, cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedDefaultGroupsAndTags(DbContext context, CancellationToken cancellationToken)
    {
        bool shouldCreateDefaultTagsAndGroups =
            !await context.Set<DisplayGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await context.Set<LogicGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await context.Set<Tag>().AnyAsync(cancellationToken).ConfigureAwait(false);

        if (!shouldCreateDefaultTagsAndGroups)
            return;

        context.AddRange(_DefaultTagProvider.GetDefaultDisplayGroups());
        context.AddRange(_DefaultTagProvider.GetDefaultLogicGroups());
        context.AddRange(_DefaultTagProvider.GetDefaultTags());

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedLevelFallbackTags(DbContext context, CancellationToken cancellationToken)
    {
        bool shouldInitializeLevelFallbackTags = !await context.Set<LevelTag>().AnyAsync(cancellationToken).ConfigureAwait(false);
        if (!shouldInitializeLevelFallbackTags)
            return;

        List<Level> levels = await context.Set<Level>().ToListAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyCollection<Tag> fallbackTags = await _FallbackTagProvider.GetFallbackTagsAsync().ConfigureAwait(false);
        foreach (Level level in levels)
        {
            level.Tags.AddRange(fallbackTags);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}