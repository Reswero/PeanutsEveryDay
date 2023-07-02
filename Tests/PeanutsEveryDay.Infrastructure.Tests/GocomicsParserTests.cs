using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class GocomicsParserTests
{
    [Fact]
    public async Task Peanuts_Comics_Parsed()
    {
        // Arrange
        GocomicsParser parser = new();

        // Act
        var enumerator = parser.ParseAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.True(parsedComic.Source == SourceType.Gocomics);
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
    }

    [Fact]
    public async Task PeanutsBegins_Comics_Parsed()
    {
        // Arrange
        GocomicsParser parser = new();

        // Act
        var enumerator = parser.ParseBeginsAsync().GetAsyncEnumerator();
        await enumerator.MoveNextAsync();

        var parsedComic = enumerator.Current;

        // Assert
        Assert.True(parsedComic.Source == SourceType.GocomicsBegins);
        Assert.NotNull(parsedComic.Url);
        Assert.NotNull(parsedComic.ImageStream);
    }
}
