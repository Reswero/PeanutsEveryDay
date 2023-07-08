using HtmlAgilityPack;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Exceptions;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Application.Modules.Parsers;
using System.Net;
using System.Runtime.CompilerServices;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Parsers;

public class GocomicsParser : IComicsParser
{
    private const int _maxAttemptsCount = 5;
    private const int _maxSkippedComics = 7;

    private const string _baseUrl = "https://www.gocomics.com/";
    private const string _peanutsUrl = $"{_baseUrl}peanuts/";
    private const string _peanutsBeginsUrl = $"{_baseUrl}peanuts-begins/";

    private const string _xpathToPeanutsImg
        = "//a[@title='Peanuts']/picture[@class='item-comic-image']/img";
    private const string _xpathToPeanutsBeginsImg
        = "//a[@title='Peanuts Begins']/picture[@class='item-comic-image']/img";

    private readonly TimeSpan _defaultRequestDelay = TimeSpan.FromMilliseconds(300);

    private readonly ParserState _state;

    public GocomicsParser(ParserState state)
    {
        _state = state;
    }

    public async IAsyncEnumerable<ParsedComic> ParseAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var comic in ParseComicsAsync(_peanutsUrl, _xpathToPeanutsImg, begins: false, cancellationToken))
        {
            yield return comic;
        }
    }

    public async IAsyncEnumerable<ParsedComic> ParseBeginsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var comic in ParseComicsAsync(_peanutsBeginsUrl, _xpathToPeanutsBeginsImg, begins: true, cancellationToken))
        {
            yield return comic;
        }
    }

    private async IAsyncEnumerable<ParsedComic> ParseComicsAsync(string baseUrl, string xpathToImg, bool begins,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        DateOnly lastComicDate = DateOnly.FromDateTime(DateTime.Now);

        using HttpClientHandler clientHandler = new() { AllowAutoRedirect = false };
        using HttpClient client = new(clientHandler);

        int attemptsCount = default;
        int skippedComics = default;

        DateOnly lastParsedComic = begins is true ? _state.LastParsedGocomicsBegins : _state.LastParsedGocomics;
        DateOnly currentComic = lastParsedComic;

        TimeSpan requestDelay = _defaultRequestDelay;

        while (lastParsedComic < lastComicDate && currentComic <= lastComicDate)
        {
            currentComic = currentComic.AddDays(1);

            var comicUrl
                = $"{baseUrl}{currentComic.Year}/{currentComic.Month}/{currentComic.Day}";

            using HttpResponseMessage status = await client.GetAsync(comicUrl, cancellationToken);

            if (status.StatusCode == HttpStatusCode.OK)
            {
                HtmlWeb web = new();
                HtmlDocument doc = await web.LoadFromWebAsync(comicUrl, cancellationToken);

                string comicImageSrc = doc.DocumentNode
                    .SelectSingleNode(xpathToImg)
                    .Attributes["src"]
                    .Value;

                using HttpResponseMessage response = await client.GetAsync(comicImageSrc, cancellationToken);
                using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                // Sometimes the image may not load correctly
                if (stream.Length > 1024)
                {
                    var sourceType = begins is true ? SourceType.GocomicsBegins : SourceType.Gocomics;

                    attemptsCount = default;
                    skippedComics = default;
                    lastParsedComic = currentComic;
                    requestDelay = _defaultRequestDelay;

                    _state.ChangeGocomics(lastParsedComic, begins);

                    yield return new ParsedComic(currentComic, sourceType, comicUrl, stream);
                }
            }
            else if (status.StatusCode == HttpStatusCode.Redirect)
            {
                attemptsCount = default;
                skippedComics++;

                if (skippedComics > _maxSkippedComics)
                {
                    yield break;
                }
            }
            else
            {
                if (attemptsCount < _maxAttemptsCount)
                {
                    attemptsCount++;
                    requestDelay = _defaultRequestDelay * Math.Pow(attemptsCount, 2);
                }
                else
                {
                    throw new AttemptsExceededException();
                }
            }

            // Avoiding ban
            await Task.Delay(requestDelay, cancellationToken);
        }
    }
}
