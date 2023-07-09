using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Infrastructure.Modules.Converters;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
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
        IComicsParser acomics = new AcomicsParser();
        IComicImageConverter converter = new ComicImageConverter();
        IComicFileSystemService fsService = new ComicFileSystemService();
        IComicsRepository repository = new ComicsRepository(_db);
        IParserStateRepository stateRepository = new ParserStateRepository(_db);

        ComicsLoaderService service = new(new[] { acomics }, converter, fsService, repository, stateRepository);

        // Act
        await service.LoadAsync(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(_db.ParserStates.Single().LastParsedAcomics > 0);
        Assert.True(_db.ParserStates.Single().LastParsedAcomicsBegins > 0);
        Assert.True(Directory.Exists(_comicsFolderPath));
    }
}
