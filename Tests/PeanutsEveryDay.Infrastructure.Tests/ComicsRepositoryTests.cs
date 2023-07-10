using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
using PeanutsEveryDay.Tests.Utils;

namespace PeanutsEveryDay.Infrastructure.Tests;

public class ComicsRepositoryTests
{
    private readonly PeanutsContext _db;

    public ComicsRepositoryTests()
    {
        int seed = Random.Shared.Next(1, 100_000);
        _db = DbUtils.CreateContext(seed);
        DbUtils.CreateDatabase(_db);
    }

    public void Dispose()
    {
        DbUtils.DropDatabase(_db);
        _db.Dispose();
    }

    [Fact]
    public async Task Comic_Added()
    {
        // Arrange
        ComicsRepository repository = new(_db);

        Comic comic = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.Acomics,
            Url = "example.com"
        };

        // Act
        await repository.AddAsync(comic);

        // Assert
        Assert.Single(_db.Comics.ToList());
        Assert.Equal(comic.PublicationDate, _db.Comics.Single().PublicationDate);
    }

    [Fact]
    public async Task ComicRange_Added()
    {
        // Arrange
        ComicsRepository repository = new(_db);

        Comic comic1 = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.Acomics,
            Url = "example.com"
        };
        Comic comic2 = new()
        {
            PublicationDate = new DateOnly(2023, 01, 01),
            Source = SourceType.Gocomics,
            Url = "example.com"
        };
        List<Comic> comics = new(new[] { comic1, comic2 });

        // Act
        await repository.AddRangeAsync(comics);

        // Assert
        Assert.Equal(comics.Count, _db.Comics.Count());
    }
}
