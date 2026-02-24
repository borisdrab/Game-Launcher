using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Launcher.DAL.Context;

public class LauncherDbContext : DbContext
{
    public LauncherDbContext(DbContextOptions<LauncherDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<LibraryEntity> Libraries => Set<LibraryEntity>();
    public DbSet<GameTitleEntity> GameTitles => Set<GameTitleEntity>();
    public DbSet<LibraryTitleEntity> LibraryTitles => Set<LibraryTitleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Composite primary key pre join tabuľku (N:M)
        modelBuilder.Entity<LibraryTitleEntity>()
            .HasKey(x => new { x.LibraryId, x.GameTitleId });
        
        // User 1..N Library
        modelBuilder.Entity<LibraryEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.Libraries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // LibraryTitle -> Library
        modelBuilder.Entity<LibraryTitleEntity>()
            .HasOne(x => x.Library)
            .WithMany(x => x.LibraryTitles)
            .HasForeignKey(x => x.LibraryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // LibraryTitle -> GameTitle
        modelBuilder.Entity<LibraryTitleEntity>()
            .HasOne(x => x.GameTitle)
            .WithMany(x => x.LibraryTitles)
            .HasForeignKey(x => x.GameTitleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Unique indexy (odporúčané)
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.UserName)
            .IsUnique();
        
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}