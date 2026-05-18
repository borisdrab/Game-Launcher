using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class GenreFacadeTests : FacadeTestsBase
{
    private readonly GenreFacade _facade;

    public GenreFacadeTests(ITestOutputHelper output) : base(output)
    {
        var ctx = DbContextFactory.CreateDbContext();
        var repository = new GenreRepository(ctx, new GenreEntityMapper());
        _facade = new GenreFacade(ctx, repository, new GenreModelMapper());
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
    public async Task Delete_NonExistentGenre_ThrowsException()
    {
        // Act - try to delete a genre that does not exist
        var action = async () => await _facade.DeleteAsync(Guid.NewGuid());

        // Assert - repository throws EntityNotFoundException
        await action.Should().ThrowAsync<EntityNotFoundException>();
    }

    // --- Query / Filter / Sort tests ---

    [Fact]
    public async Task Query_SearchByName_ReturnsMatchingGenres()
    {
        // Arrange - search for "RPG" (should match "Action-RPG", "MMORPG", "RPG")
        var query = new QueryObject();
        query.SearchTerm = "RPG";

        // Act
        var results = await _facade.GetAsync(query);

        // Assert
        var resultList = results.ToList();
        resultList.Should().HaveCount(3);
    }

    [Fact]
    public async Task Query_SearchByName_NoMatch_ReturnsEmpty()
    {
        // Arrange - search for something that does not exist
        var query = new QueryObject();
        query.SearchTerm = "NonExistentGenre";

        // Act
        var results = await _facade.GetAsync(query);

        // Assert
        var resultList = results.ToList();
        resultList.Should().BeEmpty();
    }

    [Fact]
    public async Task Query_SortByNameAscending_ReturnsInOrder()
    {
        // Arrange
        var query = new QueryObject();
        query.SortBy = "Name";
        query.SortDescending = false;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - first should be "Action-RPG", last "RPG"
        var resultList = results.ToList();
        resultList.First().Name.Should().Be("Action-RPG");
        resultList.Last().Name.Should().Be("RPG");
    }

    [Fact]
    public async Task Query_SortByNameDescending_ReturnsInReverseOrder()
    {
        // Arrange
        var query = new QueryObject();
        query.SortBy = "Name";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - first should be "RPG", last "Action-RPG"
        var resultList = results.ToList();
        resultList.First().Name.Should().Be("RPG");
        resultList.Last().Name.Should().Be("Action-RPG");
    }

    [Fact]
    public async Task Query_SearchAndSort_Combined()
    {
        // Arrange - search for "RPG" and sort descending
        var query = new QueryObject();
        query.SearchTerm = "RPG";
        query.SortBy = "Name";
        query.SortDescending = true;

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - should find 3 genres, sorted Z-A
        var resultList = results.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].Name.Should().Be("RPG");
        resultList[1].Name.Should().Be("MMORPG");
        resultList[2].Name.Should().Be("Action-RPG");
    }

    [Fact]
    public async Task Query_EmptyQuery_ReturnsAllGenres()
    {
        // Arrange - no search, no sort
        var query = new QueryObject();

        // Act
        var results = await _facade.GetAsync(query);

        // Assert - should return all 5 seeded genres
        var resultList = results.ToList();
        resultList.Should().HaveCount(5);
    }
}
