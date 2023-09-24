using EFCore.BulkExtensions;
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
        BulkConfig config = new()
        {
            PropertiesToIncludeOnUpdate = new List<string> { "" }
        };

        await _db.BulkInsertOrUpdateAsync(comics, config, cancellationToken: cancellationToken);
        await _db.BulkSaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<Comic>> GetAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var comics = await _db.Comics.Where(c => c.PublicationDate == date)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return comics;
    }
}
