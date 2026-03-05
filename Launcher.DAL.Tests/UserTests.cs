using AwesomeAssertions;
using Launcher.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Launcher.DAL.Tests;

public class UserTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task AddNew_User_Persisted()
    {
        //Arrange
        var entity = new UserEntity()
        {
            UserName = "Tomáš",
            Email = "xnovakt00@vutbr.cz",
            DisplayName = "tom123",
            AvatarUrl = ""
        };
        
        //Act
        LauncherDbContextSut.Add(entity);
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Users.FirstAsync(userEntity => userEntity.Id == entity.Id);
        entityFromDb.Should().NotBeNull();
        entityFromDb.Should().BeEquivalentTo(entity, options => options
            .Excluding(user => user.Libraries)
            .Excluding(user => user.Reviews)
            .Excluding(user => user.UserAchievements)
        );
    }

    [Fact]
    public async Task UpdateExisting_User_Persisted()
    {
        //Arrange
        var existingUser = await LauncherDbContextSut.Users.FirstAsync(userEntity => userEntity.UserName == "Ondrej");
        
        //Pre-Assert to check if the user has been correctly retrieved from the database
        existingUser.Should().NotBeNull();
        
        //Act
        const string newDisplayName = "Ondrjuu_"; 
        existingUser.DisplayName = newDisplayName;
        await LauncherDbContextSut.SaveChangesAsync();
        
        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var entityFromDb = await dbx.Users.FirstAsync(userEntity => userEntity.Id == existingUser.Id);
        entityFromDb.DisplayName.Should().Be(newDisplayName);
    }
}