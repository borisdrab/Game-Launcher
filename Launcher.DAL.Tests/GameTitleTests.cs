using Launcher.DAL.Entities;
using Launcher.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class GameTitleTests(ITestOutputHelper output) : DbContextTestsBase(output)
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
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.GameTitles.FirstAsync(titleEntity => titleEntity.Id == entity.Id );
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(e => e.Achievements)
            .Excluding(e => e.GameTitlePlatforms)
            .Excluding(e => e.LibraryTitles)
            .Excluding(e => e.Reviews)
        );
    }

    [Fact]
    public async Task GetAll_GameTitles_ContainsSeededEldenRing()
    {
        //Act
        var entities = await LauncherDbContextSut.GameTitles.ToArrayAsync();
        
        //Assert
        entities.Should().ContainEquivalentOf(GameTitleSeeds.EldenRing, options => options
            .Excluding(e => e.Achievements)
            .Excluding(e => e.GameTitlePlatforms)
            .Excluding(e => e.LibraryTitles)
            .Excluding(e => e.Reviews)
        );
    }

    [Fact]
    public async Task Update_GameTitle_ChangesPersisted()
    {
        //Arrange
        const string originalName = "Terrarie";
        var entity = new GameTitleEntity
        {
            Name = originalName,
            Description = "Dig, fight, explore, build! Nothing is impossible in this action-packed adventure game. Four Pack also available!",
            PegiRating = 12,
            PriceCents = 1075,
            ReleaseDate = new DateTime(2011, 5, 16),
            Publisher = "Re-Logic",
            IsAvailable = true,
            CoverImageUrl = "https://upload.wikimedia.org/wikipedia/en/1/1a/Terraria_Steam_artwork.jpg"
        };
        
        LauncherDbContextSut.GameTitles.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        LauncherDbContextSut.ChangeTracker.Clear();
        
        //Act
        await using var actDbx = await DbContextFactory.CreateDbContextAsync();
        var gameToUpdate = await actDbx.GameTitles.FindAsync(entity.Id);
        
        gameToUpdate.Should().NotBeNull();
        gameToUpdate.Name = "Terraria";
        gameToUpdate.PriceCents = 975;
        
        await actDbx.SaveChangesAsync();
        
        //Assert
        await using var assertDbx = await DbContextFactory.CreateDbContextAsync();
        var updatedGame = await assertDbx.GameTitles.FindAsync(entity.Id);
        
        updatedGame.Should().NotBeNull();
        updatedGame.Name.Should().Be("Terraria");
        updatedGame.PriceCents.Should().Be(975);
        updatedGame.Description.Should().Be(entity.Description);
    }
}