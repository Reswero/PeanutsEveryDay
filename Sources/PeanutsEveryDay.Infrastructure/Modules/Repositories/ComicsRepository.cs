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
        var comicDb = CreateEntry(comic);

        await _db.AddAsync(comicDb, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IReadOnlyCollection<Comic> comics, CancellationToken cancellationToken = default)
    {
        var comicsDb = comics.Select(CreateEntry).ToList();

        await _db.AddRangeAsync(comicsDb, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private Data.Models.Comic CreateEntry(Comic comic)
    {
        return new()
        {
            PublicationDate = comic.PublicationDate,
            Source = comic.Source,
            Url = comic.Url
        };
    }
}
