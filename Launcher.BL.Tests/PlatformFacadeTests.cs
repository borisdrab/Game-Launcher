using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class PlatformFacadeTests : FacadeTestsBase
{
    private readonly PlatformFacade _facade;

    public PlatformFacadeTests(ITestOutputHelper output) : base(output)
    {
        // Create the mapper and facade that we will test
        var mapper = new PlatformModelMapper();
        _facade = new PlatformFacade(mapper, DbContextFactory);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededPlatforms()
    {
        // Act - get all platforms from the database
        var platforms = await _facade.GetAsync();

        // Assert - there should be 3 seeded platforms (PC, Xbox, PlayStation 5)
        var platformList = platforms.ToList();
        platformList.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetById_ReturnsCorrectPlatform()
    {
        // Act - get the "PC" platform by its seeded Id
        var platform = await _facade.GetAsync(PlatformSeeds.Pc.Id);

        // Assert - we should get back the PC platform
        platform.Should().NotBeNull();
        platform!.Name.Should().Be("PC");
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNull()
    {
        // Act - try to get a platform that does not exist
        var platform = await _facade.GetAsync(Guid.NewGuid());

        // Assert - should be null
        platform.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewPlatform_Persisted()
    {
        // Arrange - create a new platform model
        var newPlatform = new PlatformDetailModel();
        newPlatform.Name = "Nintendo Switch";

        // Act - save it through the facade
        var savedId = await _facade.SaveAsync(newPlatform);

        // Assert - we should be able to get it back from the database
        var platformFromDb = await _facade.GetAsync(savedId);
        platformFromDb.Should().NotBeNull();
        platformFromDb!.Name.Should().Be("Nintendo Switch");
    }

    [Fact]
    public async Task Save_UpdateExistingPlatform_NameChanged()
    {
        // Arrange - take the seeded Xbox platform and change its name
        var platformToUpdate = new PlatformDetailModel();
        platformToUpdate.Id = PlatformSeeds.Xbox.Id;
        platformToUpdate.Name = "Xbox Series X";

        // Act - save the updated platform
        await _facade.SaveAsync(platformToUpdate);

        // Assert - the name should be updated in the database
        var updatedPlatform = await _facade.GetAsync(PlatformSeeds.Xbox.Id);
        updatedPlatform.Should().NotBeNull();
        updatedPlatform!.Name.Should().Be("Xbox Series X");
    }

    [Fact]
    public async Task Delete_ExistingPlatform_Removed()
    {
        // Act - delete the seeded "PlayStation 5" platform
        await _facade.DeleteAsync(PlatformSeeds.PlayStation5.Id);

        // Assert - it should no longer exist in the database
        var deletedPlatform = await _facade.GetAsync(PlatformSeeds.PlayStation5.Id);
        deletedPlatform.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistentPlatform_DoesNothing()
    {
        // Act - try to delete a platform that does not exist (should not throw)
        var action = async () => await _facade.DeleteAsync(Guid.NewGuid());

        // Assert - no exception should be thrown
        await action.Should().NotThrowAsync();
    }
}
