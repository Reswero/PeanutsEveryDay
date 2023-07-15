using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;

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
        var state = await _db.ParserStates.AsNoTracking().SingleOrDefaultAsync(cancellationToken) ?? new();

        return state;
    }

    public async Task AddOrUpdateAsync(ParserState state, CancellationToken cancellationToken = default)
    {
        var stateExists = await _db.ParserStates.AsNoTracking().SingleOrDefaultAsync(cancellationToken) is not null;

        if (stateExists)
        {
            _db.Update(state);
        }
        else
        {
            await _db.AddAsync(state, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
