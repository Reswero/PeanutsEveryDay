using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Persistence;
using PeanutsEveryDay.Tests.Utils;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class UsersRepositoryTests
{
    private readonly PeanutsContext _db;

    public UsersRepositoryTests()
    {
        _db = DbUtils.CreateContext();
        DbUtils.CreateDatabase(_db);
    }

    public void Dispose()
    {
        DbUtils.DropDatabase(_db);
        _db.Dispose();
    }

    [Fact]
    public async Task User_Added()
    {
        // Arrange
        UsersRepository repository = new(_db);

        User user = User.Create(1, "Test", null);

        // Act
        await repository.AddAsync(user);

        // Assert
        Assert.Single(_db.Users.ToList());
        Assert.Single(_db.UsersSettings.ToList());
        Assert.Single(_db.UsersProgress.ToList());
        Assert.Equal(user.Id, _db.Users.Single().Id);
        Assert.Equal(user.FirstName, _db.Users.Single().FirstName);
    }

    [Fact]
    public async Task User_Returned()
    {
        // Arrange
        UsersRepository repository = new(_db);

        UserProgress progressDb = new() { UserId = 1 };
        UserSettings settingsDb = new() { UserId = 1 };
        User userDb = new() { Id = 1, FirstName = "Test", Progress = progressDb, Settings = settingsDb };
        await _db.Users.AddAsync(userDb);
        await _db.SaveChangesAsync();

        DbUtils.ClearTracker(_db);

        // Act
        var user = await repository.GetAsync(userDb.Id);

        // Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Progress);
        Assert.NotNull(user.Settings);
        Assert.Equal(userDb.Id, user.Id);
        Assert.Equal(userDb.FirstName, user.FirstName);
    }

    [Fact]
    public async Task User_Updated()
    {
        // Arrange
        UsersRepository repository = new(_db);

        UserProgress progressDb = new() { UserId = 1 };
        UserSettings settingsDb = new() { UserId = 1 };
        User userDb = new() { Id = 1, FirstName = "Test", Progress = progressDb, Settings = settingsDb };
        await _db.Users.AddAsync(userDb);
        await _db.SaveChangesAsync();

        DbUtils.ClearTracker(_db);

        UserProgress progress = new() { UserId = userDb.Id, TotalComicsWatched = 20 };
        UserSettings settings = new() { UserId = userDb.Id };
        User user = new() { Id = userDb.Id, FirstName = "Test2", Progress = progressDb, Settings = settingsDb };

        // Act
        await repository.UpdateAsync(user);

        // Assert
        Assert.Equal(user.FirstName, _db.Users.Single().FirstName);
        Assert.Equal(user.Progress.TotalComicsWatched, _db.UsersProgress.Single().TotalComicsWatched);
    }
}
