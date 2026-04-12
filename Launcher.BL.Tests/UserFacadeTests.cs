using AwesomeAssertions;
using Launcher.BL.Facades;
using Launcher.BL.Mappers;
using Launcher.BL.Models;
using Launcher.BL.Repositories;
using Launcher.DAL.Seeds;
using Xunit.Abstractions;

namespace Launcher.BL.Tests;

public class UserFacadeTests : FacadeTestsBase
{
    private readonly UserFacade _facade;

    public UserFacadeTests(ITestOutputHelper output) : base(output)
    {
        var ctx = DbContextFactory.CreateDbContext();
        var repository = new UserRepository(ctx, new UserEntityMapper());
        _facade = new UserFacade(ctx, repository, new UserModelMapper());
    }

    [Fact]
    public async Task GetAsync_ReturnsAllSeededUsers()
    {
        // Act
        var users = await _facade.GetAsync();
        
        // Assert
        users.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsCorrectDetailModel()
    {
        // Act
        var user = await _facade.GetAsync(UserSeeds.Stepan.Id);
        
        // Assert
        user.Should().NotBeNull();
        user.Id.Should().Be(UserSeeds.Stepan.Id);
        user.UserName.Should().Be(UserSeeds.Stepan.UserName);
        user.Email.Should().Be(UserSeeds.Stepan.Email);
    }

    [Fact]
    public async Task GetAsync_ById_NonExistingId_ReturnsNull()
    {
        // Act
        var user = await _facade.GetAsync(Guid.NewGuid());
        
        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task Save_NewUser_Persisted()
    {
        // Arrange
        var newUser = new UserDetailModel
        {
            UserName = "NewTestUser",
            Email = "newuser@example.com",
            DisplayName = "Test User",
            AvatarUrl = ""
        };

        // Act
        var savedId = await _facade.SaveAsync(newUser);
        
        // Assert
        var userFromDb = await _facade.GetAsync(savedId);
        userFromDb.Should().NotBeNull();
        userFromDb.UserName.Should().Be("NewTestUser");
        userFromDb.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task Save_UpdateExistingUser_DisplayChangedName()
    {
        // Arrange
        var userToUpdate = new UserDetailModel
        {
            Id = UserSeeds.Boris.Id,
            UserName = UserSeeds.Boris.UserName,
            Email = UserSeeds.Boris.Email,
            DisplayName = "Updated Boris",
            AvatarUrl = UserSeeds.Boris.AvatarUrl
        };
        
        // Act
        await _facade.SaveAsync(userToUpdate);
        
        // Assert
        var updated = await _facade.GetAsync(UserSeeds.Boris.Id);
        updated.Should().NotBeNull();
        updated.DisplayName.Should().Be("Updated Boris");
    }

    [Fact]
    public async Task Delete_ExistingUser_Removed()
    {
        // Arrange
        var newUser = new UserDetailModel
        {
            UserName = "ToDelete",
            Email = "delete@test.com",
            DisplayName = "Delete Me",
            AvatarUrl = ""
        };
        var savedId = await _facade.SaveAsync(newUser);
        
        // Act
        await _facade.DeleteAsync(savedId);
        
        // Assert
        var deleted = await _facade.GetAsync(savedId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_SearchByUserName_ReturnsOnlyMatching()
    {
        // Act
        var results = (await _facade.GetAsync("Boris", null, true)).ToList();
        
        // Assert
        results.Should().HaveCount(1);
        results[0].UserName.Should().Be("Boris");
    }

    [Fact]
    public async Task GetAsync_SortByDisplayName_ReturnsOrdered()
    {
        // Act
        var results = (await _facade.GetAsync(null, "displayname", true)).ToList();
        
        // Assert
        var displayNames = results.Select(u => u.DisplayName).ToList();
        displayNames.Should().BeInAscendingOrder();
    }
    
    [Fact]
    public async Task GetAsync_SelectUserFromList_DetailMatchesListEntry()
    {
        // Arrange
        var userList = (await _facade.GetAsync()).ToList();
        var selectedFromList = userList.First(u => u.UserName == "Boris");

        // Act
        var detail = await _facade.GetAsync(selectedFromList.Id);

        // Assert
        detail.Should().NotBeNull();
        detail!.Id.Should().Be(selectedFromList.Id);
        detail.UserName.Should().Be(selectedFromList.UserName);
        detail.DisplayName.Should().Be(selectedFromList.DisplayName);
        detail.Email.Should().Be(UserSeeds.Boris.Email);
    }
}