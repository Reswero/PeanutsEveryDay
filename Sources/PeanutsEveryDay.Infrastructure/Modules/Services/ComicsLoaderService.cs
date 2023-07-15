using Microsoft.Extensions.Logging;
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

    private readonly BlockingCollection<Comic> _comicsBag = new();

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

    public async Task LoadAsync(TimeSpan? executionDuration = null, CancellationToken cancellationToken = default)
    {
        Task saveTask = Task.Run(SaveComicsAsync);
        var state = await _stateRepository.GetAsync(cancellationToken);

        _logger.LogInformation("Parser state loaded (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
            "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
            state.LastParsedAcomics, state.LastParsedAcomicsBegins, state.LastParsedGocomics, state.LastParsedGocomicsBegins);

        List<Task> loadTasks = new(_parsers.Length * 2);
        foreach (var parser in _parsers)
        {
            parser.SetState(state);

            var comicsTask = Task.Run(async () => await LoadComicsAsync(parser.ParseAsync(cancellationToken)), cancellationToken);
            var beginsComicsTask = Task.Run(async () => await LoadComicsAsync(parser.ParseBeginsAsync(cancellationToken)), cancellationToken);

            loadTasks.Add(comicsTask);
            loadTasks.Add(beginsComicsTask);

            _logger.LogInformation("Parser {Name} starts working.", parser.GetType().Name);
        }

        try
        {
            var timeout = executionDuration is null ? -1 : (int)executionDuration.Value.TotalMilliseconds;
            Task.WaitAll(loadTasks.ToArray(), timeout, cancellationToken);
        }
        finally
        {
            _comicsBag.CompleteAdding();
            await saveTask;
            await _stateRepository.AddOrUpdateAsync(state, cancellationToken);

            _logger.LogInformation("Parser state saved (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
                "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
                state.LastParsedAcomics, state.LastParsedAcomicsBegins, state.LastParsedGocomics, state.LastParsedGocomicsBegins);
        }
    }

    private async Task LoadComicsAsync(IAsyncEnumerable<ParsedComic> comics)
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
                

                _logger.LogTrace("Comic parsed (pubDate='{PublicationDate}', src={Source}, url='{Url}').",
                    comic.PublicationDate, comic.Source, comic.Url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parser exception occured. {Error}", ex.Message);
        }
    }

    private async Task SaveComicsAsync()
    {
        int totalComicsAdded = default;

        List<Comic> comics = new(_parsers.Length * 4);
        while (_comicsBag.IsCompleted == false)
        {
            while (_comicsBag.Count > 0)
                comics.Add(_comicsBag.Take());

            if (comics.Count > 0)
            {
                await _comicsRepository.AddRangeAsync(comics);

                _logger.LogTrace("{Count} comics added to repository.", comics.Count);
                totalComicsAdded += comics.Count;

                comics.Clear();
            }

            await Task.Delay(500);
        }

        _logger.LogInformation("Total {Count} comics added.", totalComicsAdded);
    }
}
