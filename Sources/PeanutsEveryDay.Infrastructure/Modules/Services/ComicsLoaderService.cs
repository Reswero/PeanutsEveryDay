using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsLoaderService : IComicsLoaderService
{
    private readonly IComicsParser[] _parsers;
    private readonly IComicImageConverter _converter;
    private readonly IComicFileSystemService _fileSystemService;
    private readonly IComicsRepository _repository;

    public ComicsLoaderService(IComicsParser[] parsers, IComicImageConverter converter,
        IComicFileSystemService fileSystemService, IComicsRepository repository)
    {
        _parsers = parsers;
        _converter = converter;
        _fileSystemService = fileSystemService;
        _repository = repository;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        List<Task> loadTasks = new(_parsers.Length * 2);
        foreach (var parser in _parsers)
        {
            var comicsTask = Task.Run(async () => await LoadComics(parser.ParseAsync(cancellationToken)), cancellationToken);
            var beginsComicsTask = Task.Run(async () => await LoadComics(parser.ParseBeginsAsync(cancellationToken)), cancellationToken);

            loadTasks.Add(comicsTask);
            loadTasks.Add(beginsComicsTask);
        }

        Task.WaitAll(loadTasks.ToArray(), cancellationToken);
    }

    private async Task LoadComics(IAsyncEnumerable<ParsedComic> comics)
    {
        await foreach (var parsedComic in comics)
        {
            var imgStream = await _converter.ConvertFromStripToSquareAsync(parsedComic.ImageStream);
            await _fileSystemService.SaveImage(imgStream, parsedComic.PublicationDate, parsedComic.Source);

            Comic comic = new()
            {
                PublicationDate = parsedComic.PublicationDate,
                Source = parsedComic.Source,
                Url = parsedComic.Url
            };
            await _repository.AddAsync(comic);
        }
    }
}
