using Launcher.DAL.Context;
using Launcher.DAL.Entities;
using Launcher.DAL.Factories;

namespace Launcher.DAL.Tests;

public class GameTitleTests : DbContextTestsBase
{
    [Fact]
    public async Task AddNew_GameTitle_Persisted()
    {
        //Arrange
        var entity = new GameTitleEntity
        {
            Name = "Clair Obscur: Expedition 33",
            Description = "Lead the members of Expedition 33 on their quest to destroy the Paintress so that she can never paint death again. Explore a world of wonders inspired by Belle Époque France and battle unique enemies in this turn-based RPG with real-time mechanics.",
            PegiRating = 18,
            PriceCents = 4999,
            ReleaseDate = new DateTime(2025, 4, 24),
            Publisher = "Kepler Interactive",
            IsAvailable = true,
            CoverImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/5/5a/Clair_Obscur%2C_Expedition_33_Cover_1.webp/250px-Clair_Obscur%2C_Expedition_33_Cover_1.webp.png"
        };
        
        //Act
        LauncherDbContextSut.GameTitles.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = base.DbContextFactory.CreateDbContext();
        var entityFromDb = dbx.GameTitles.First(titleEntity => titleEntity.Id == entity.Id );
        Assert.Equal(entity.Id, entityFromDb.Id);
    }
}