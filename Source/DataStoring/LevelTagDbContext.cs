using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace DataStoring;

public class LevelTagDbContext : DbContext
{
    public DbSet<Level> Levels { get; set; }

    public DbSet<TagGroup> TagGroups { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<LevelTag> LevelTags { get; set; }

    public DbSet<SteamUser> SteamUsers { get; set; }

    public LevelTagDbContext(DbContextOptions<LevelTagDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureLevelEntity(modelBuilder);
        ConfigureTagEntity(modelBuilder);
        ConfigureSteamUserEntity(modelBuilder);
    }

    private void ConfigureLevelEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Level>()
            .HasKey(level => level.Id);

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

    private void ConfigureTagEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .HasKey(tag => tag.Id);

        modelBuilder.Entity<Tag>()
            .HasIndex(tag => tag.Name)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .Property(tag => tag.Name)
            .HasMaxLength(30);
    }

    private void ConfigureSteamUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SteamUser>()
            .HasKey(user => user.Id);

        modelBuilder.Entity<SteamUser>()
            .Property(user => user.Id)
            .ValueGeneratedNever();
    }
}