using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class GenreTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Genre_Persisted()
    {
        //Arrange
        var entity = new GenreEntity() { Name = "JRPG" };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Genres.FirstAsync(genreEntity => genreEntity.Id == entity.Id);
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(genre => genre.Games)
        );
    }
    
    [Fact]
    public async Task Link_Genre_ToNewGame()
    {
        //Arrange
        var mmorpgGenre = await LauncherDbContextSut.Genres.FirstAsync(genre => genre.Name == "MMORPG");
        var firstPersonShooterGenre = await LauncherDbContextSut.Genres.FirstAsync(genre => genre.Name == "First Person Shooter");
        var newGame = new GameTitleEntity()
        {
            Name = "Destiny 2",
            PriceCents = 0
        };
        
        newGame.Genres.Add(mmorpgGenre);
        newGame.Genres.Add(firstPersonShooterGenre);
        
        //Act
        LauncherDbContextSut.GameTitles.Add(newGame);
        await LauncherDbContextSut.SaveChangesAsync();
        
        LauncherDbContextSut.ChangeTracker.Clear();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var savedGame = await dbx.GameTitles
            .Include(g => g.Genres)
            .FirstAsync(g => g.Id == newGame.Id);
        savedGame.Genres.Should().HaveCount(2);
        savedGame.Genres.Should().Contain(g => g.Name == "MMORPG");
        savedGame.Genres.Should().Contain(g => g.Name == "First Person Shooter");
    }
}