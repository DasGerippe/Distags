using DataClasses;
using Microsoft.EntityFrameworkCore;

namespace DataStoring;

public class LevelTagDbContext : DbContext
{
    public DbSet<Level> Levels { get; set; }

    public DbSet<TagGroup> TagGroups { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<LevelTag> LevelTags { get; set; }

    public DbSet<Creator> Creators { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserSession> UserSessions { get; set; }

    public LevelTagDbContext(DbContextOptions<LevelTagDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureLevelEntity(modelBuilder);
        ConfigureTagEntity(modelBuilder);
        ConfigureCreatorEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
        ConfigureUserSessionEntity(modelBuilder);
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

    private void ConfigureCreatorEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Creator>()
            .HasKey(creator => creator.Id);

        modelBuilder.Entity<Creator>()
            .Property(creator => creator.Id)
            .ValueGeneratedNever();
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(user => user.Id)
            .ValueGeneratedNever();
    }

    private void ConfigureUserSessionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSession>()
            .HasKey(session => session.Token);
    }
}