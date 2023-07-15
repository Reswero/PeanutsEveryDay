using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsService : IComicsService
{
    private readonly IComicsRepository _repository;
    private readonly IComicFileSystemService _fileSystemService;

    public ComicsService(IComicsRepository repository, IComicFileSystemService fileSystemService)
    {
        _repository = repository;
        _fileSystemService = fileSystemService;
    }

    public async Task<ParsedComic?> GetComicAsync(DateOnly date, SourceType sources, CancellationToken cancellationToken = default)
    {
        var comics = await _repository.GetAsync(date, cancellationToken);
        
        var comic = GetComicByHighSource(comics, sources);
        if (comic is not null)
        {
            var stream = await _fileSystemService.GetImageAsync(date, comic.Source, cancellationToken);
            return new(comic.PublicationDate, comic.Source, comic.Url, stream);
        }

        return null;
    }

    private Comic? GetComicByHighSource(IReadOnlyCollection<Comic> comics, SourceType sources)
    {
        Comic? comic = null;
        if (sources.HasFlag(SourceType.AcomicsBegins))
        {
            comic = comics.FirstOrDefault(c => c.Source == SourceType.AcomicsBegins);
            if (comic is not null) return comic;
        }
        if (sources.HasFlag(SourceType.Acomics))
        {
            comic = comics.FirstOrDefault(c => c.Source == SourceType.Acomics);
            if (comic is not null) return comic;
        }
        if (sources.HasFlag(SourceType.GocomicsBegins))
        {
            comic = comics.FirstOrDefault(c => c.Source == SourceType.GocomicsBegins);
            if (comic is not null) return comic;
        }
        if (sources.HasFlag(SourceType.Gocomics))
        {
            comic = comics.FirstOrDefault(c => c.Source == SourceType.Gocomics);
            if (comic is not null) return comic;
        }

        return comic;
    }
}
