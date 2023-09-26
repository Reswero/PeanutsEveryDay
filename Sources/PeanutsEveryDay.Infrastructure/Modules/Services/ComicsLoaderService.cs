using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Application.Modules.Comics;
using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;
using System.Collections.Concurrent;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsLoaderService : IComicsLoaderService
{
    private readonly ILogger<ComicsLoaderService> _logger;

    private readonly IComicsParser[] _parsers;
    private readonly IComicImageConverter _converter;
    private readonly IComicFileSystemService _fileSystemService;
    private readonly IComicsRepository _comicsRepository;
    private readonly IParserStateRepository _stateRepository;

    private readonly TimeSpan _loadingDuration = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _restDuration = TimeSpan.FromMinutes(10);
    private readonly int _savingInterval = (int) TimeSpan.FromSeconds(5).TotalMilliseconds;

    private ParserState _state;
    private BlockingCollection<Comic> _comicsBag;

    public ComicsLoaderService(ILogger<ComicsLoaderService> logger, IEnumerable<IComicsParser> parsers,
        IComicImageConverter converter, IComicFileSystemService fileSystemService,
        IComicsRepository comicsRepository, IParserStateRepository stateRepository)
    {
        _logger = logger;
        _parsers = parsers.ToArray();
        _converter = converter;
        _fileSystemService = fileSystemService;
        _comicsRepository = comicsRepository;
        _stateRepository = stateRepository;
    }

    public async Task StartLoadingAsync(CancellationToken cancellationToken = default)
    {
        _state = await _stateRepository.GetAsync(cancellationToken);

        while (true)
        {
            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_loadingDuration);

            _logger.LogInformation("Comics loading started.");

            _comicsBag = new();
            await LoadAsync(cts.Token);

            _logger.LogInformation("Comics loading ended.");

            if (cancellationToken.IsCancellationRequested)
                return;

            await Task.Delay(_restDuration, cancellationToken);
        }
    }

    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Task saveTask = Task.Run(SaveComicsAsync);

        _logger.LogInformation("Parser state loaded (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
            "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
            _state.LastParsedAcomics, _state.LastParsedAcomicsBegins, _state.LastParsedGocomics, _state.LastParsedGocomicsBegins);

        List<Task> loadTasks = new(_parsers.Length * 2);
        foreach (var parser in _parsers)
        {
            parser.SetState(_state);

            var comicsTask = Task.Run(async () => await LoadComicsAsync(parser.ParseAsync(), cancellationToken));
            var beginsComicsTask = Task.Run(async () => await LoadComicsAsync(parser.ParseBeginsAsync(), cancellationToken));

            loadTasks.Add(comicsTask);
            loadTasks.Add(beginsComicsTask);

            _logger.LogInformation("Parser {Name} starts working.", parser.GetType().Name);
        }

        Task.WaitAll(loadTasks.ToArray());

        _comicsBag.CompleteAdding();
        await saveTask;
        await _stateRepository.AddOrUpdateAsync(_state);

        _logger.LogInformation("Parser state saved (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
            "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
            _state.LastParsedAcomics, _state.LastParsedAcomicsBegins, _state.LastParsedGocomics, _state.LastParsedGocomicsBegins);
    }

    private async Task LoadComicsAsync(IAsyncEnumerable<ParsedComic> comics, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var parsedComic in comics)
            {
                var imgStream = await _converter.ConvertFromStripToSquareAsync(parsedComic.ImageStream);
                await _fileSystemService.SaveImageAsync(imgStream, parsedComic.PublicationDate, parsedComic.Source);

                Comic comic = new()
                {
                    PublicationDate = parsedComic.PublicationDate,
                    Source = parsedComic.Source,
                    Url = parsedComic.Url
                };
                _comicsBag.Add(comic);

                _logger.LogTrace("Comic parsed (pubDate='{PublicationDate}', src='{Source}', url='{Url}').",
                    comic.PublicationDate, comic.Source, comic.Url);

                if (cancellationToken.IsCancellationRequested)
                    return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while loading comics. {Error}", ex.Message);
        }
    }

    private async Task SaveComicsAsync()
    {
        int totalComicsAdded = default;
        ComicsComparer comparer = new();

        List<Comic> comics = new(_parsers.Length * 10);
        while (_comicsBag.IsCompleted == false)
        {
            while (_comicsBag.Count > 0)
                comics.Add(_comicsBag.Take());

            if (comics.Count > 0)
            {
                var uniqueComics = comics.Distinct(comparer).ToList();
                await _comicsRepository.AddRangeAsync(uniqueComics);

                _logger.LogTrace("{Count} comics added to repository.", comics.Count);
                totalComicsAdded += comics.Count;

                comics.Clear();
            }

            await Task.Delay(_savingInterval);
        }

        _logger.LogInformation("Total {Count} comics added.", totalComicsAdded);
    }
}
