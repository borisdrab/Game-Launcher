using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class GenreFacadeTests : FacadeTestsBase
{
    private readonly GenreFacade _facade;

    public GenreFacadeTests(ITestOutputHelper output) : base(output)
    {
        // Create the mapper and facade that we will test
        var mapper = new GenreModelMapper();
        _facade = new GenreFacade(mapper, DbContextFactory);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededGenres()
    {
        // Act - get all genres from the database
        var genres = await _facade.GetAsync();

        // Assert - there should be 5 seeded genres
        var genreList = genres.ToList();
        genreList.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetById_ReturnsCorrectGenre()
    {
        // Act - get the "RPG" genre by its seeded Id
        var genre = await _facade.GetAsync(GenreSeeds.Rpg.Id);

        // Assert - we should get back the RPG genre
        genre.Should().NotBeNull();
        genre!.Name.Should().Be("RPG");
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        // Act - try to get a genre that does not exist
        var genre = await _facade.GetAsync(Guid.NewGuid());

        // Assert - should be null
        genre.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewGenre_Persisted()
    {
        // Arrange - create a new genre model
        var newGenre = new GenreDetailModel();
        newGenre.Name = "Strategy";

        // Act - save it through the facade
        var savedId = await _facade.SaveAsync(newGenre);

        // Assert - we should be able to get it back from the database
        var genreFromDb = await _facade.GetAsync(savedId);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be("Strategy");
    }

    [Fact]
    public async Task Save_UpdateExistingGenre_NameChanged()
    {
        // Arrange - take the seeded RPG genre and change its name
        var genreToUpdate = new GenreDetailModel();
        genreToUpdate.Id = GenreSeeds.Rpg.Id;
        genreToUpdate.Name = "Role Playing Game";

        // Act - save the updated genre
        await _facade.SaveAsync(genreToUpdate);

        // Assert - the name should be updated in the database
        var updatedGenre = await _facade.GetAsync(GenreSeeds.Rpg.Id);
        updatedGenre.Should().NotBeNull();
        updatedGenre!.Name.Should().Be("Role Playing Game");
    }

    [Fact]
    public async Task Delete_ExistingGenre_Removed()
    {
        // Act - delete the seeded "MMORPG" genre
        await _facade.DeleteAsync(GenreSeeds.Mmorpg.Id);

        // Assert - it should no longer exist in the database
        var deletedGenre = await _facade.GetAsync(GenreSeeds.Mmorpg.Id);
        deletedGenre.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistentGenre_DoesNothing()
    {
        // Act - try to delete a genre that does not exist (should not throw)
        var action = async () => await _facade.DeleteAsync(Guid.NewGuid());

        // Assert - no exception should be thrown
        await action.Should().NotThrowAsync();
    }
}
