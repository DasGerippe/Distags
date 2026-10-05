using Distags.Core;
using Distags.Core.Commands.ImportOfficialLevels;
using Distags.Core.Entities;
using Distags.Persistence.Seeding.OfficialLevels;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence.Seeding;

internal class DataSeeder
{
    private readonly DefaultTagProvider _DefaultTagProvider;
    private readonly OfficialLevelsProvider _OfficialLevelsProvider;
    private readonly IRequestDispatcher _RequestDispatcher;

    public DataSeeder(
        DefaultTagProvider defaultTagProvider,
        OfficialLevelsProvider officialLevelsProvider,
        IRequestDispatcher requestDispatcher)
    {
        _DefaultTagProvider = defaultTagProvider;
        _OfficialLevelsProvider = officialLevelsProvider;
        _RequestDispatcher = requestDispatcher;
    }

    public void Seed(DbContext context, bool dataStoreChanged)
    {
        SeedAsync(context, dataStoreChanged, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SeedAsync(DbContext context, bool dataStoreChanged, CancellationToken cancellationToken)
    {
        await SeedDefaultGroupsAndTags(context, cancellationToken).ConfigureAwait(false);
        await SeedOfficialLevelsAsync().ConfigureAwait(false);
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

    private async Task SeedOfficialLevelsAsync()
    {
        ImportOfficialLevelsCommand importOfficialLevelsCommand = new ImportOfficialLevelsCommand
        {
            ScrapedOfficialLevels = _OfficialLevelsProvider.GetOfficialLevels(),
        };
        await _RequestDispatcher.DispatchAsync(importOfficialLevelsCommand).ConfigureAwait(false);
    }
}
