using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Converters;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;
using PeanutsEveryDay.Infrastructure.Modules.Services;
using PeanutsEveryDay.Tests.Utils;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class ComicsLoaderServiceTests
{
    private const string _comicsFolderPath = "comics";
    private readonly PeanutsContext _db;

    public ComicsLoaderServiceTests()
    {
        _db = DbUtils.CreateContext();
        DbUtils.CreateDatabase(_db);
    }

    public void Dispose()
    {
        DbUtils.DropDatabase(_db);
        _db.Dispose();
        Directory.Delete(_comicsFolderPath, true);
    }

    [Fact]
    public async Task Comics_Loaded()
    {
        // Arrange
        ParserState state = new();

        IComicsParser acomics = new AcomicsParser(state);
        IComicImageConverter converter = new ComicImageConverter();
        IComicFileSystemService fsService = new ComicFileSystemService();
        IComicsRepository repository = new ComicsRepository(_db);

        ComicsLoaderService service = new(new[] { acomics }, converter, fsService, repository);

        CancellationTokenSource cts = new();

        // Act
        // Not optimal solution. Maybe should change IComicsParser to Mock realisation
        Task.Run(async () => await service.LoadAsync(cts.Token));
        await Task.Delay(5000);
        cts.Cancel();

        // Assert
        Assert.True(state.LastParsedAcomics > 0);
        Assert.True(state.LastParsedAcomicsBegins > 0);
        Assert.True(Directory.Exists(_comicsFolderPath));
    }
}
