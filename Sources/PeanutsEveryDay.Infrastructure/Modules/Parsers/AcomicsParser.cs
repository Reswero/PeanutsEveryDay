﻿using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Exceptions;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Domain.Models;
using System.Net;
using System.Runtime.CompilerServices;

namespace PeanutsEveryDay.Infrastructure.Modules.Parsers;

public class AcomicsParser : IComicsParser
{
    private const int _maxAttemptsCount = 5;

    private const string _baseUrl = "https://acomics.ru/";
    private const string _peanutsUrl = $"{_baseUrl}~peanuts/";
    private const string _peanutsBeginsUrl = $"{_baseUrl}~peanutsbegins/";

    private const string _xpathToImage = "//img[@id='mainImage']";
    private const string _xpathToDate = "//span[@class='issueName']";
    private const string _xpathToRootUrl = "//div[@class='description']//a";
    private const string _xpathToComicsNumber = "//span[@class='issueNumber']";

    private readonly ILogger<AcomicsParser> _logger;
    private readonly TimeSpan _defaultRequestDelay = TimeSpan.FromMilliseconds(10);

    private ParserState _state = new();

    public AcomicsParser(ILogger<AcomicsParser> logger)
    {
        _logger = logger;
    }

    public void SetState(ParserState state)
    {
        _state = state;
    }

    public async IAsyncEnumerable<ParsedComic> ParseAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var comic in ParseComicsAsync(_peanutsUrl, begins: false, cancellationToken))
        {
            yield return comic;
        }
    }

    public async IAsyncEnumerable<ParsedComic> ParseBeginsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var comic in ParseComicsAsync(_peanutsBeginsUrl, begins: true, cancellationToken))
        {
            yield return comic;
        }
    }

    private async IAsyncEnumerable<ParsedComic> ParseComicsAsync(string baseUrl, bool begins,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int lastComicNumber = await GetLastComicNumberAsync(baseUrl, cancellationToken);

        using HttpClient client = new();

        int attemptsCount = default;
        int lastParsedComic = begins is true ? _state.LastParsedAcomicsBegins : _state.LastParsedAcomics;

        TimeSpan requestDelay = _defaultRequestDelay;

        while (lastParsedComic < lastComicNumber)
        {
            bool parsed = false;
            ParsedComic? parsedComic = null;

            try
            {
                var comicUrl = $"{baseUrl}{lastParsedComic + 1}";
                using HttpResponseMessage response = await client.GetAsync(comicUrl, cancellationToken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var (date, src) = await GetPublicationDateAndSrcAsync(comicUrl, begins, cancellationToken);

                    string comicImageUrl = $"{_baseUrl}{src}";

                    using HttpResponseMessage imageResponse = await client.GetAsync(comicImageUrl, cancellationToken);
                    using Stream stream = await imageResponse.Content.ReadAsStreamAsync(cancellationToken);

                    var sourceType = begins is true ? SourceType.AcomicsBegins : SourceType.Acomics;

                    attemptsCount = default;
                    lastParsedComic++;
                    requestDelay = _defaultRequestDelay;

                    _state.ChangeAcomics(lastParsedComic, begins);

                    parsedComic = new ParsedComic(date, sourceType, comicUrl, stream);
                    parsed = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while parsing Acomics. {Error}", ex.Message);
            }

            if (parsed is true)
            {
                yield return parsedComic!;
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

            await Task.Delay(requestDelay, cancellationToken);
        }
    }

    private async Task<int> GetLastComicNumberAsync(string url, CancellationToken cancellationToken)
    {
        int comicNumber = default;

        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            HtmlWeb web = new();
            HtmlDocument doc = await web.LoadFromWebAsync(url, cancellationToken);

            string nodeText = doc.DocumentNode
                .SelectSingleNode(_xpathToComicsNumber)
                .InnerText
                .Split('/')[0];

            int.TryParse(nodeText, out comicNumber);
        }

        return comicNumber;
    }

    private async Task<(DateOnly, string)> GetPublicationDateAndSrcAsync(string comicUrl, bool begins,
        CancellationToken cancellationToken)
    {
        HtmlWeb web = new();
        HtmlDocument doc = await web.LoadFromWebAsync(comicUrl, cancellationToken);

        string comicImageSrc = doc.DocumentNode
            .SelectSingleNode(_xpathToImage)
            .Attributes["src"]
            .Value;

        if (begins is true)
        {
            string rootUrl = doc.DocumentNode
            .SelectNodes(_xpathToRootUrl)[1]
                .Attributes["href"]
                .Value;

            doc = await web.LoadFromWebAsync(rootUrl, cancellationToken);
        }

        string comicDateText = doc.DocumentNode
                .SelectSingleNode(_xpathToDate)
                .InnerText
                .Trim('/');

        DateOnly comicDate = DateOnly.Parse(comicDateText);

        return (comicDate, comicImageSrc);
    }
}
