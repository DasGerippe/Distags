using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace DataStoring;

public class LevelTagDbContext : DbContext
{
    public LevelTagDbContext(DbContextOptions<LevelTagDbContext> options)
        : base(options)
    {
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
        ConfigureLevelEntity(modelBuilder);
        ConfigureLevelLockEntity(modelBuilder);
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
}