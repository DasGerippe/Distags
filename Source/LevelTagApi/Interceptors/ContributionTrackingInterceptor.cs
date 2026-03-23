using DataClasses;
using DataStoring;
using LevelTagApi.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LevelTagApi.Interceptors;

public class ContributionTrackingInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<ContributionTrackingInterceptor> _Logger;

    public ContributionTrackingInterceptor(ILogger<ContributionTrackingInterceptor> logger)
    {
        _Logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CreateContributionsForChangedLevelTags(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await CreateContributionsForChangedLevelTags(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task CreateContributionsForChangedLevelTags(DbContext? dbContext)
    {
        if (dbContext is not LevelTagDbContext)
            return;

        List<LevelTag> addedLevelTags = dbContext.ChangeTracker.Entries<LevelTag>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => entry.Entity)
            .ToList();

        List<LevelTag> deletedLevelTags = dbContext.ChangeTracker.Entries<LevelTag>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Select(entry => entry.Entity)
            .ToList();

        if (addedLevelTags.Count == 0 && deletedLevelTags.Count == 0)
            return;

        try
        {
            IContributionManager contributionManager = dbContext.GetService<IContributionManager>();
            await contributionManager.CreateContributionsAsync(addedLevelTags, deletedLevelTags);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "An error occurred while creating contributions for added and deleted level tags.");
        }
    }
}