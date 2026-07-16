using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence.Seeding;

internal class DataSeeder
{
    public void Seed(DbContext context, bool dataStoreChanged)
    {
        SeedAsync(context, dataStoreChanged, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SeedAsync(DbContext context, bool dataStoreChanged, CancellationToken cancellationToken)
    {
        bool shouldCreateDefaultTagsAndGroups =
            !await context.Set<DisplayGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await context.Set<LogicGroup>().AnyAsync(cancellationToken).ConfigureAwait(false) &&
            !await context.Set<Tag>().AnyAsync(cancellationToken).ConfigureAwait(false);

        if (shouldCreateDefaultTagsAndGroups)
        {
            DefaultTagProvider defaultTagProvider = new DefaultTagProvider();

            context.AddRange(defaultTagProvider.GetDefaultDisplayGroups());
            context.AddRange(defaultTagProvider.GetDefaultLogicGroups());
            context.AddRange(defaultTagProvider.GetDefaultTags());

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}