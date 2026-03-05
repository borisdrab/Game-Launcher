using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class LibraryTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_Library_Persisted()
    {
        //Arrange
        var seededUser = await LauncherDbContextSut.Users.FirstAsync(userEntity => userEntity.UserName == "Štěpán");
        var entity = new LibraryEntity()
        {
            Name = "10/10",
            UserId = seededUser.Id
        };

        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Libraries.FirstAsync(libraryEntity => libraryEntity.Name == "10/10");
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(libraryEntity => libraryEntity.User)
            .Excluding(libraryEntity => libraryEntity.LibraryTitles)
        );
        entityFromDb.UserId.Should().Be(seededUser.Id);
    }
    
    [Fact]
    public async Task Library_Contains_SeededLibraryTitles()
    {
        //Arrange
        var targetLibraryUser = await LauncherDbContextSut.Users.FirstAsync(userEntity => userEntity.DisplayName == "Schpagy");
        var targetLibrary = await LauncherDbContextSut.Libraries.FirstAsync(libraryEntity => libraryEntity.UserId == targetLibraryUser.Id);
        
        //Act
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var libraryFromDb =  await dbx.Libraries
            .Include(libraryEntity => libraryEntity.LibraryTitles)
            .ThenInclude(libraryEntity => libraryEntity.GameTitle)
            .FirstAsync(libraryEntity => libraryEntity.Id == targetLibrary.Id);
        
        //Assert
        libraryFromDb.Should().NotBeNull();
        libraryFromDb.LibraryTitles.Should().NotBeEmpty().And.HaveCountGreaterThanOrEqualTo(1);
        
        var libraryTitle = libraryFromDb.LibraryTitles.First();
        libraryTitle.LibraryId.Should().Be(targetLibrary.Id);
        libraryTitle.GameTitle.Should().NotBeNull();
        libraryTitle.GameTitle.Name.Should().Be("ELDEN RING");
    }
}