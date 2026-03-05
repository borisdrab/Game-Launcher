using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class GameTitleGenreTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task TheWitcher3_IsLinkedTo_BothOpenWorldAndRpg()
    {
        //Arrange
        var openWorldGenre = await LauncherDbContextSut.Genres.FirstAsync(genre => genre.Name == "Open World");
        var rpgGenre = await LauncherDbContextSut.Genres.FirstAsync(genre => genre.Name == "RPG");
        
        //Act
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var theWitcher3FromDb = await dbx.GameTitles
            .Include(gameTitleEntity => gameTitleEntity.GameTitleGenres)
            .FirstAsync(gameTitleEntity => gameTitleEntity.Name == "The Witcher 3: Wild Hunt");
        
        //Assert
        theWitcher3FromDb.Should().NotBeNull();
        theWitcher3FromDb.GameTitleGenres.Should().HaveCount(2);
        theWitcher3FromDb.GameTitleGenres.Should().Contain(gameTitleGenreEntity => gameTitleGenreEntity.GenreId == openWorldGenre.Id);
        theWitcher3FromDb.GameTitleGenres.Should().Contain(gameTitleGenreEntity => gameTitleGenreEntity.GenreId == rpgGenre.Id);
    }
}