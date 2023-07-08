using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Services;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsLoaderService : IComicsLoaderService
{
    private readonly IComicsParser[] _parsers;
    private readonly IComicImageConverter _converter;
    private readonly IComicFileSystemService _fileSystemService;

    public ComicsLoaderService(IComicsParser[] parsers, IComicImageConverter converter,
        IComicFileSystemService fileSystemService)
    {
        _parsers = parsers;
        _converter = converter;
        _fileSystemService = fileSystemService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        List<Task> loadTasks = new(_parsers.Length * 2);
        foreach (var parser in _parsers)
        {
            var comicsTask = Task.Run(() => LoadComics(parser.ParseAsync(cancellationToken)), cancellationToken);
            var beginsComicsTask = Task.Run(() => LoadComics(parser.ParseBeginsAsync(cancellationToken)), cancellationToken);

            loadTasks.Add(comicsTask);
            loadTasks.Add(beginsComicsTask);
        }

        Task.WaitAll(loadTasks.ToArray(), cancellationToken);
    }

    private async void LoadComics(IAsyncEnumerable<ParsedComic> comics)
    {
        await foreach (var comic in comics)
        {
            await _converter.ConvertFromStripToSquareAsync(comic.ImageStream);
            await _fileSystemService.SaveImage(comic.ImageStream, comic.PublicationDate, comic.Source);
        }
    }
}
