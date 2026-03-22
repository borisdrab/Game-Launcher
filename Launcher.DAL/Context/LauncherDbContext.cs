using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Context;

public class LauncherDbContext(DbContextOptions<LauncherDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<LibraryEntity> Libraries => Set<LibraryEntity>();
    public DbSet<GameTitleEntity> GameTitles => Set<GameTitleEntity>();
    public DbSet<LibraryTitleEntity> LibraryTitles => Set<LibraryTitleEntity>();

    public DbSet<GenreEntity> Genres => Set<GenreEntity>();
    
    public DbSet<PlatformEntity> Platforms => Set<PlatformEntity>();
    public DbSet<GameTitlePlatformEntity> GameTitlePlatforms => Set<GameTitlePlatformEntity>();
    
    public DbSet<GameTitleGenreEntity> GameTitleGenres => Set<GameTitleGenreEntity>();
    
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    
    public DbSet<AchievementEntity> Achievements => Set<AchievementEntity>();
    public DbSet<UserAchievementEntity> UserAchievements => Set<UserAchievementEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LibraryTitleEntity>()
            .HasKey(x => new { x.LibraryId, x.GameTitleId });

        modelBuilder.Entity<LibraryTitleEntity>()
            .HasOne(x => x.Library)
            .WithMany(x => x.LibraryTitles)
            .HasForeignKey(x => x.LibraryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LibraryTitleEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x => x.LibraryTitles)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.UserName)
            .IsUnique();

        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<UserEntity>()
            .Property(x => x.UserName)
            .HasMaxLength(64)
            .IsRequired();

        modelBuilder.Entity<UserEntity>()
            .Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        modelBuilder.Entity<UserEntity>()
            .Property(x => x.DisplayName)
            .HasMaxLength(128);

        modelBuilder.Entity<UserEntity>()
            .Property(x => x.AvatarUrl)
            .HasMaxLength(2048);
        
        
        
        modelBuilder.Entity<GameTitleEntity>()
            .HasIndex(x => x.Name);

        modelBuilder.Entity<GameTitleEntity>()
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        modelBuilder.Entity<GameTitleEntity>()
            .Property(x => x.Description)
            .HasMaxLength(2000);

        modelBuilder.Entity<GameTitleEntity>()
            .Property(x => x.PriceCents)
            .IsRequired();

        modelBuilder.Entity<GameTitleEntity>()
            .Property(x => x.CoverImageUrl)
            .HasMaxLength(2048);

        modelBuilder.Entity<GameTitleEntity>()
            .Property(x => x.Publisher)
            .HasMaxLength(128);
        

        modelBuilder.Entity<GameTitlePlatformEntity>()
            .HasKey(x => new { x.GameTitleId, x.PlatformId });
        
        modelBuilder.Entity<GameTitlePlatformEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x => x.GameTitlePlatforms)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GameTitlePlatformEntity>()
            .HasOne(x => x.Platform)
            .WithMany(x => x.GameTitlePlatforms)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PlatformEntity>()
            .HasIndex(x => x.Name)
            .IsUnique();
        
        modelBuilder.Entity<PlatformEntity>()
            .Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();
        
        
        
        modelBuilder.Entity<ReviewEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ReviewEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ReviewEntity>()
            .HasIndex(x => new { x.UserId, x.GameTitleId})
            .IsUnique();
        
        modelBuilder.Entity<ReviewEntity>()
            .Property(x => x.Rating)
            .IsRequired();
        
        modelBuilder.Entity<ReviewEntity>()
            .Property(x => x.Text)
            .HasMaxLength(2000);
        
        
        
        modelBuilder.Entity<AchievementEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x => x.Achievements)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAchievementEntity>()
            .HasKey(x => new { x.UserId, x.AchievementId });
        
        modelBuilder.Entity<UserAchievementEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserAchievements)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserAchievementEntity>()
            .HasOne(x => x.Achievement)
            .WithMany(x => x.UserAchievements)
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<AchievementEntity>()
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();
        
        modelBuilder.Entity<AchievementEntity>()
            .Property(x => x.Description)
            .HasMaxLength(505);
        
        
        modelBuilder.Entity<GenreEntity>()
            .HasIndex(x => x.Name)
            .IsUnique();
        
        modelBuilder.Entity<GenreEntity>()
            .Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();
        
        
        
        modelBuilder.Entity<LibraryEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.Libraries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<LibraryEntity>()
            .Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();
        
        
        
        modelBuilder.Entity<GameTitleGenreEntity>()
            .HasKey(x => new { x.GameTitleId, x.GenreId });
        
        modelBuilder.Entity<GameTitleGenreEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x=> x.GameTitleGenres)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<GameTitleGenreEntity>()
            .HasOne(x => x.Genre)
            .WithMany(x => x.GamesTitleGenres)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}