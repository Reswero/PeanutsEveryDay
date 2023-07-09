using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;

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
        Data.Models.Comic comicDb = new()
        {
            PublicationDate = comic.PublicationDate,
            Source = comic.Source,
            Url = comic.Url
        };

        await _db.AddAsync(comicDb, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
