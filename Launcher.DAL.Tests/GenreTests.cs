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
            .Excluding(genre => genre.GamesTitleGenres)
        );
    }
}