using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;

namespace PeanutsEveryDay.Infrastructure.Modules.Repositories;

public class ComicsRepository : IComicsRepository
{
    private readonly PeanutsContext _db;

    public ComicsRepository(PeanutsContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Comic comic, CancellationToken cancellationToken = default)
    {
        await _db.AddAsync(comic, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IReadOnlyCollection<Comic> comics, CancellationToken cancellationToken = default)
    {
        await _db.AddRangeAsync(comics, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Comic?> GetAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var comic = await _db.Comics.Where(c => c.PublicationDate == date && c.Source == Abstraction.SourceType.AcomicsBegins)
            .FirstOrDefaultAsync(cancellationToken);

        return comic;
    }
}
