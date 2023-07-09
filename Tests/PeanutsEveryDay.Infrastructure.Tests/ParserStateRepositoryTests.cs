using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
using PeanutsEveryDay.Tests.Utils;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class ParserStateRepositoryTests
{
    private readonly PeanutsContext _db;

    public ParserStateRepositoryTests()
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
    public async Task ParserState_Added()
    {
        // Arrange
        ParserStateRepository repository = new(_db);

        ParserState state = new();

        // Act
        await repository.AddOrUpdateAsync(state);

        // Assert
        Assert.Single(_db.ParserStates.ToList());
    }

    [Fact]
    public async Task ParserState_Updated()
    {
        // Arrange
        ParserStateRepository repository = new(_db);

        Data.Models.ParserState stateDb = new();
        await _db.ParserStates.AddAsync(stateDb);
        await _db.SaveChangesAsync();
        DbUtils.ClearTracker(_db);

        ParserState state = new() { LastParsedAcomics = 2 };

        // Act
        await repository.AddOrUpdateAsync(state);

        // Assert
        Assert.Single(_db.ParserStates.ToList());
        Assert.Equal(state.LastParsedAcomics, _db.ParserStates.Single().LastParsedAcomics);
    }

    [Fact]
    public async Task ParserState_Returned()
    {
        // Arrange
        ParserStateRepository repository = new(_db);

        Data.Models.ParserState stateDb = new();
        await _db.ParserStates.AddAsync(stateDb);
        await _db.SaveChangesAsync();
        DbUtils.ClearTracker(_db);

        // Act
        var state = await repository.GetAsync();

        // Assert
        Assert.Equal(state.LastParsedAcomics, _db.ParserStates.Single().LastParsedAcomics);
    }
}
