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
        if (!context.Set<Tag>().Any() && !context.Set<TagGroup>().Any())
        {
            IEnumerable<TagGroup> defaultTagTree = new DefaultTagProvider().GetDefaultTagTree();
            context.AddRange(defaultTagTree);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}