using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicsLoaderService : IComicsLoaderService
{
    private readonly ILogger<ComicsLoaderService> _logger;

    private readonly IComicsParser[] _parsers;
    private readonly IComicImageConverter _converter;
    private readonly IComicFileSystemService _fileSystemService;
    private readonly IComicsRepository _repository;
    private readonly IParserStateRepository _stateRepository;

    public ComicsLoaderService(ILogger<ComicsLoaderService> logger, IComicsParser[] parsers,
        IComicImageConverter converter, IComicFileSystemService fileSystemService,
        IComicsRepository repository, IParserStateRepository stateRepository)
    {
        _logger = logger;
        _parsers = parsers;
        _converter = converter;
        _fileSystemService = fileSystemService;
        _repository = repository;
        _stateRepository = stateRepository;
    }

    public async Task LoadAsync(TimeSpan? executionDuration = null, CancellationToken cancellationToken = default)
    {
        var state = await _stateRepository.GetAsync(cancellationToken);

        _logger.LogInformation("Parser state loaded (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
            "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
            state.LastParsedAcomics, state.LastParsedAcomicsBegins, state.LastParsedGocomics, state.LastParsedGocomicsBegins);

        List<Task> loadTasks = new(_parsers.Length * 2);
        foreach (var parser in _parsers)
        {
            parser.SetState(state);

            var comicsTask = Task.Run(async () => await LoadComics(parser.ParseAsync(cancellationToken)), cancellationToken);
            var beginsComicsTask = Task.Run(async () => await LoadComics(parser.ParseBeginsAsync(cancellationToken)), cancellationToken);

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
            await _stateRepository.AddOrUpdateAsync(state, cancellationToken);

            _logger.LogInformation("Parser state saved (acomics='{acomics}', acomicsBegins='{acomicsBegins}', " +
                "gocomics='{gocomics}', gocomicsBegins='{gocomicsBegins}').",
                state.LastParsedAcomics, state.LastParsedAcomicsBegins, state.LastParsedGocomics, state.LastParsedGocomicsBegins);
        }
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

            _logger.LogTrace("Comic parsed (pubDate='{PublicationDate}', src={Source}, url='{Url}').",
                comic.PublicationDate, comic.Source, comic.Url);
        }
    }
}
