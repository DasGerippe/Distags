using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace DataStoring;

public class LevelTagDbContext : DbContext
{
    public DbSet<App> Apps { get; set; }

    public DbSet<Contribution> Contributions { get; set; }

    public DbSet<Creator> Creators { get; set; }

    public DbSet<Level> Levels { get; set; }

    public DbSet<LevelLock> LevelLocks { get; set; }

    public DbSet<LevelTag> LevelTags { get; set; }

    public DbSet<Setting> Settings { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<TagGroup> TagGroups { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserBan> UserBans { get; set; }

    public DbSet<UserSession> UserSessions { get; set; }

    public LevelTagDbContext(DbContextOptions<LevelTagDbContext> options)
        : base(options)
    {
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