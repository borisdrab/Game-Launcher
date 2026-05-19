using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class LibraryFacadeTests : FacadeTestsBase
{
    private readonly LibraryFacade _facade;

    public LibraryFacadeTests(ITestOutputHelper output) : base(output)
    {
        var ctx = DbContextFactory.CreateDbContext();
        var repository = new LibraryRepository(ctx, new LibraryEntityMapper());
        _facade = new LibraryFacade(ctx, repository, new LibraryModelMapper());
    }

    [Fact]
    public async Task GetAsync_ReturnsAllSeededLibraries()
    {
        // Act
        var libraries = await _facade.GetAsync();
        
        // Assert
        libraries.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsCorrectDetailModel()
    {
        // Act
        var library = await _facade.GetAsync(LibrarySeeds.SchpagysCompleted.Id);
        
        // Assert
        library.Should().NotBeNull();
        library!.Id.Should().Be(LibrarySeeds.SchpagysCompleted.Id);
        library.Name.Should().Be(LibrarySeeds.SchpagysCompleted.Name);
        library.UserId.Should().Be(LibrarySeeds.SchpagysCompleted.UserId);
        library.LibraryTitles.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAsync_ById_NonExistingId_ReturnsNull()
    {
        // Act
        var library = await _facade.GetAsync(Guid.NewGuid());
        
        // Assert
        library.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewLibrary_Persisted()
    {
        // Arrange
        var newLibrary = new LibraryDetailModel
        {
            Name = "New Test Library",
            UserId = UserSeeds.Stepan.Id
        };

        // Act
        var savedId = await _facade.SaveAsync(newLibrary);
        
        // Assert
        var libFromDb = await _facade.GetAsync(savedId);
        libFromDb.Should().NotBeNull();
        libFromDb!.Name.Should().Be("New Test Library");
        libFromDb.UserId.Should().Be(UserSeeds.Stepan.Id);
    }

    [Fact]
    public async Task Save_UpdateExistingLibrary_NameChanged()
    {
        // Arrange
        var libToUpdate = new LibraryDetailModel
        {
            Id = LibrarySeeds.BorissFavourite.Id,
            Name = "Updated Name",
            UserId = LibrarySeeds.BorissFavourite.UserId
        };
        
        // Act
        await _facade.SaveAsync(libToUpdate);
        
        // Assert
        var updated = await _facade.GetAsync(LibrarySeeds.BorissFavourite.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Delete_ExistingLibrary_Removed()
    {
        // Arrange
        var newLib = new LibraryDetailModel
        {
            Name = "ToDelete",
            UserId = UserSeeds.Jan.Id
        };
        var savedId = await _facade.SaveAsync(newLib);
        
        // Act
        await _facade.DeleteAsync(savedId);
        
        // Assert
        var deleted = await _facade.GetAsync(savedId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task FilterAsync_ByUserId_ReturnsUserLibraryWithGames()
    {
        // Act
        var result = await _facade.FilterAsync(UserSeeds.Jan.Id, null, null, true);
        
        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(UserSeeds.Jan.Id);
        result.LibraryTitles.Should().NotBeEmpty();
        result.LibraryTitles.Any(lt => lt.GameTitle!.Name == GameTitleSeeds.EldenRing.Name).Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_ByName_ReturnsOnlyMatchingGames()
    {
        // Act
        var result = await _facade.FilterAsync(UserSeeds.Jan.Id, "Elden", null, true);
        
        // Assert
        result.LibraryTitles.Should().HaveCount(1);
        result.LibraryTitles.First().GameTitle!.Name.Should().ContainEquivalentOf("Elden");
    }

    [Fact]
    public async Task FilterAsync_ByGenre_ReturnsOnlyMatchingGames()
    {
        // Act
        var result = await _facade.FilterAsync(UserSeeds.Jan.Id, null, null, true, "RPG");
        
        // Assert
        // Elden Ring is Action RPG
        result.LibraryTitles.Should().NotBeEmpty();
        result.LibraryTitles.All(lt => lt.GameTitle!.GameTitleGenres.Any(gtg => gtg.Genre!.Name == "RPG")).Should().BeTrue();
    }

    [Fact]
    public async Task FilterAsync_SortByNameAscending_ReturnsOrdered()
    {
        // Act
        var result = await _facade.FilterAsync(UserSeeds.Jan.Id, null, "name", true);
        
        // Assert
        var names = result.LibraryTitles.Select(lt => lt.GameTitle!.Name).ToList();
        names.Should().BeInAscendingOrder();
    }
}
