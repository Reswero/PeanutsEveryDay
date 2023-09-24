using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class GocomicsParserTests
{
    [Fact]
    public async Task Peanuts_Comics_Parsed()
    {
        // Arrange
        var logFactory = LoggerFactory.Create(cfg => cfg.SetMinimumLevel(LogLevel.Trace));
        var logger = logFactory.CreateLogger<GocomicsParser>();

        ParserState state = new();
        GocomicsParser parser = new(logger);
        parser.SetState(state);

        var initComicDate = state.LastParsedGocomics;

        // Act
        var enumerator = parser.ParseAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
        Assert.True(parsedComic.Source == SourceType.Gocomics);
        Assert.True(state.LastParsedGocomics.DayNumber - initComicDate.DayNumber == 1);
    }

    [Fact]
    public async Task PeanutsBegins_Comics_Parsed()
    {
        // Arrange
        var logFactory = LoggerFactory.Create(cfg => cfg.SetMinimumLevel(LogLevel.Trace));
        var logger = logFactory.CreateLogger<GocomicsParser>();

        ParserState state = new();
        GocomicsParser parser = new(logger);
        parser.SetState(state);

        var initComicDate = state.LastParsedGocomicsBegins;

        // Act
        var enumerator = parser.ParseBeginsAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
        Assert.True(parsedComic.Source == SourceType.GocomicsBegins);
        Assert.True(state.LastParsedGocomicsBegins.DayNumber - initComicDate.DayNumber == 1);
    }
}
