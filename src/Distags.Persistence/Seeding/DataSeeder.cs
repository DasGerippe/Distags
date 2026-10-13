using Distags.Core;
using Distags.Core.Commands.ImportOfficialLevels;
using Distags.Persistence.Seeding.OfficialLevels;
using Distags.Persistence.Seeding.Tags;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence.Seeding;

internal class DataSeeder
{
    private readonly TagSeeder _TagSeeder;
    private readonly OfficialLevelsProvider _OfficialLevelsProvider;
    private readonly IRequestDispatcher _RequestDispatcher;

    public DataSeeder(
        TagSeeder tagSeeder,
        OfficialLevelsProvider officialLevelsProvider,
        IRequestDispatcher requestDispatcher)
    {
        _TagSeeder = tagSeeder;
        _OfficialLevelsProvider = officialLevelsProvider;
        _RequestDispatcher = requestDispatcher;
    }

    public void Seed(DbContext context, bool dataStoreChanged)
    {
        SeedAsync(context, dataStoreChanged, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SeedAsync(DbContext context, bool dataStoreChanged, CancellationToken cancellationToken)
    {
        await _TagSeeder.SeedGroupsAndTagsAsync(context, cancellationToken).ConfigureAwait(false);
        await SeedOfficialLevelsAsync().ConfigureAwait(false);
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
