using Distags.Core.Entities;
using Distags.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Persistence;

public class DistagsDbContext : Distags.Core.DistagsDbContext
{
    public DistagsDbContext(DbContextOptions<DistagsDbContext> options)
        : base(options)
    {
        SavingChanges += DistagsDbContext_SavingChanges;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new TablePluralizationConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAppEntity(modelBuilder);
        ConfigureContributionEntity(modelBuilder);
        ConfigureCreatorEntity(modelBuilder);
        ConfigureDisplayGroupEntity(modelBuilder);
        ConfigureLevelEntity(modelBuilder);
        ConfigureLevelLockEntity(modelBuilder);
        ConfigureLogicGroupEntity(modelBuilder);
        ConfigureSettingEntity(modelBuilder);
        ConfigureTagEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
        ConfigureUserBanEntity(modelBuilder);
        ConfigureUserSessionEntity(modelBuilder);
    }

    private void ConfigureAppEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>()
            .HasIndex(app => app.Name)
            .IsUnique();

        modelBuilder.Entity<App>()
            .HasIndex(app => app.ApiKey)
            .IsUnique();
    }

    private void ConfigureContributionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contribution>()
            .ComplexCollection(contribution => contribution.Changes, changes => changes.ToJson());
    }

    private void ConfigureCreatorEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Creator>()
            .Property(creator => creator.Id)
            .ValueGeneratedNever();
    }

    private void ConfigureDisplayGroupEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DisplayGroup>()
            .HasIndex(displayGroup => displayGroup.Name)
            .IsUnique();
    }

    private void ConfigureLevelEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Level>()
            .Property(level => level.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Level>()
            .HasMany(level => level.Tags)
            .WithMany(tag => tag.Levels)
            .UsingEntity<LevelTag>();

        modelBuilder.Entity<Level>()
            .HasOne(level => level.Creator)
            .WithMany(user => user.CreatedLevels)
            .IsRequired(false);
    }

    private void ConfigureLevelLockEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LevelLock>()
            .HasKey(levelLock => levelLock.LevelId);

        modelBuilder.Entity<LevelLock>()
            .Property(levelLock => levelLock.LevelId)
            .ValueGeneratedNever();
    }

    private void ConfigureLogicGroupEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogicGroup>()
            .HasIndex(logicGroup => logicGroup.Name)
            .IsUnique();
    }

    private void ConfigureSettingEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setting>()
            .HasKey(setting => setting.Key);
    }

    private void ConfigureTagEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .HasIndex(tag => tag.Name)
            .IsUnique();
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.SteamId)
            .IsUnique();
    }

    private void ConfigureUserBanEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBan>()
            .HasKey(userBan => userBan.SteamId);

        modelBuilder.Entity<UserBan>()
            .Property(userBan => userBan.SteamId)
            .ValueGeneratedNever();
    }

    private void ConfigureUserSessionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>()
            .HasKey(session => session.Token);
    }

    private void DistagsDbContext_SavingChanges(object? sender, SavingChangesEventArgs e)
    {
        List<LevelLock> newLevelLocks = ChangeTracker.Entries<Level>()
            .Where(entry => entry.State == EntityState.Added)
            .Select(entry => new LevelLock { LevelId = entry.Entity.Id })
            .ToList();

        AddRange(newLevelLocks);
    }
}