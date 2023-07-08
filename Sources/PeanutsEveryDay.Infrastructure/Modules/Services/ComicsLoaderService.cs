using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Services;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsLoaderService : IComicsLoaderService
{
    private readonly IComicsParser[] _parsers;
    private readonly IComicImageConverter _converter;

    public ComicsLoaderService(IComicsParser[] parsers, IComicImageConverter converter)
    {
        _parsers = parsers;
        _converter = converter;
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
        }
    }
}
