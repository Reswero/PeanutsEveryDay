using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class AcomicsParserTests
{
    [Fact]
    public async void Peanuts_Comic_Parsed()
    {
        // Arrange
        ParserState state = new();
        AcomicsParser parser = new();
        parser.SetState(state);

        var initComicNumber = state.LastParsedAcomics;

        // Act
        var enumerator = parser.ParseAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
        Assert.True(parsedComic.Source == SourceType.Acomics);
        Assert.True(state.LastParsedAcomics - initComicNumber == 1);
    }

    [Fact]
    public async void PeanutsBegins_Comic_Parsed()
    {
        // Arrange
        ParserState state = new();
        AcomicsParser parser = new();
        parser.SetState(state);

        var initComicNumber = state.LastParsedAcomicsBegins;

        // Act
        var enumerator = parser.ParseBeginsAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
        Assert.True(parsedComic.Source == SourceType.AcomicsBegins);
        Assert.True(state.LastParsedAcomicsBegins - initComicNumber == 1);
    }
}