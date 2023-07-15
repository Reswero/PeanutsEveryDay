using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;
using PeanutsEveryDay.Tests.Utils;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class PeanutsContextTests
{
    private readonly PeanutsContext _db;

    public PeanutsContextTests()
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
    public async Task Comic_Added()
    {
        // Arrange
        Comic comic = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.Acomics,
            Url = "example.com"
        };

        // Act
        await _db.AddAsync(comic);
        await _db.SaveChangesAsync();

        // Assert
        Assert.Single(_db.Comics.ToList());
    }

    [Fact]
    public async Task Comics_Added()
    {
        // Arrange
        Comic comic1 = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.Acomics,
            Url = "example.com"
        };

        Comic comic2 = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.AcomicsBegins,
            Url = "example.com"
        };

        var comics = new[] { comic1, comic2 };

        // Act
        await _db.AddRangeAsync(comics);
        await _db.SaveChangesAsync();

        // Assert
        Assert.Equal(comics.Length, _db.Comics.ToList().Count);
    }

    [Fact]
    public async Task ParserState_Added()
    {
        // Arrange
        ParserState state = new();

        // Act
        await _db.AddAsync(state);
        await _db.SaveChangesAsync();

        // Assert
        Assert.Single(_db.ParserStates.ToList());
        Assert.Equal(state.LastParsedGocomics, _db.ParserStates.Single().LastParsedGocomics);
    }

    [Fact]
    public async Task User_Added()
    {
        // Arrange
        UserProgress progressDb = new() { UserId = 1 };
        UserSettings settingsDb = new() { UserId = 1 };
        User userDb = new() { Id = 1, FirstName = "Test", Progress = progressDb, Settings = settingsDb };

        // Act
        await _db.AddAsync(userDb);
        await _db.SaveChangesAsync();

        // Assert
        Assert.Single(_db.Users.ToList());
        Assert.Single(_db.UsersProgress.ToList());
        Assert.Equal(userDb.Id, _db.UsersProgress.Single().UserId);
        Assert.Single(_db.UsersSettings.ToList());
        Assert.Equal(userDb.Id, _db.UsersSettings.Single().UserId);
    }
}