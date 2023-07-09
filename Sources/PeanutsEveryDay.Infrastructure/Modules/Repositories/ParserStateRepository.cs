using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Repositories;

public class ParserStateRepository : IParserStateRepository
{
    private readonly PeanutsContext _db;

    public ParserStateRepository(PeanutsContext db)
    {
        _db = db;
    }

    public async Task<ParserState> GetAsync(CancellationToken cancellationToken = default)
    {
        var stateDb = await _db.ParserStates.AsNoTracking().SingleOrDefaultAsync(cancellationToken) ?? new();

        return new()
        {
            LastParsedAcomics = stateDb.LastParsedAcomics,
            LastParsedAcomicsBegins = stateDb.LastParsedAcomicsBegins,
            LastParsedGocomics = stateDb.LastParsedGocomics,
            LastParsedGocomicsBegins = stateDb.LastParsedGocomicsBegins
        };
    }

    public async Task AddOrUpdateAsync(ParserState state, CancellationToken cancellationToken = default)
    {
        Data.Models.ParserState stateDb = new()
        {
            Id = 1,
            LastParsedAcomics = state.LastParsedAcomics,
            LastParsedAcomicsBegins = state.LastParsedAcomicsBegins,
            LastParsedGocomics = state.LastParsedGocomics,
            LastParsedGocomicsBegins = state.LastParsedGocomicsBegins
        };

        var stateExists = await _db.ParserStates.AsNoTracking().SingleOrDefaultAsync(cancellationToken) is not null;

        if (stateExists)
        {
            _db.Update(stateDb);
        }
        else
        {
            await _db.AddAsync(stateDb, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
