using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class AcomicsParserTests
{
    [Fact]
    public async void Peanuts_Comic_Parsed()
    {
        // Arrange
        AcomicsParser parser = new();

        // Act
        var enumerator = parser.ParseAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.True(parsedComic.Source == SourceType.Acomics);
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
    }

    [Fact]
    public async void PeanutsBegins_Comic_Parsed()
    {
        // Arrange
        AcomicsParser parser = new();

        // Act
        var enumerator = parser.ParseBeginsAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.True(parsedComic.Source == SourceType.AcomicsBegins);
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
    }
}