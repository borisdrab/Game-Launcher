using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class LibraryTitleTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_LibraryTitle_Persistent()
    {
        //Arrange
        var seededLibrary = await LauncherDbContextSut.Libraries.FirstAsync(libraryEntity => libraryEntity.Name == "Completed");
        var seededGame = await LauncherDbContextSut.GameTitles.FirstAsync(gameEntity => gameEntity.Name == "The Witcher 3: Wild Hunt");
        var entity = new LibraryTitleEntity()
        {
            LibraryId = seededLibrary.Id,
            GameTitleId =  seededGame.Id,
            AddedAt = DateTime.Now,
            IsFavorite = false,
            PriceCentsAtPurchase = 2999
        };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.LibraryTitles.FirstAsync(libraryTitleEntity => libraryTitleEntity.LibraryId == entity.LibraryId && libraryTitleEntity.GameTitleId == entity.GameTitleId);
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(libraryTitleEntity => libraryTitleEntity.Library)
            .Excluding(libraryTitleEntity => libraryTitleEntity.GameTitle)
        );
    }
}