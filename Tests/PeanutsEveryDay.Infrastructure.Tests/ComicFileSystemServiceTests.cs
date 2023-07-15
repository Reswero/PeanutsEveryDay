using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Infrastructure.Modules.Services;

namespace PeanutsEveryDay.Infrastructure.Tests;

[CollectionDefinition("comicsFS", DisableParallelization = true)]
public class ComicFileSystemServiceTests
{
    private const string _baseFolder = "comics/";
    public readonly string FolderPath = $"{_baseFolder}acomics/peanuts/";

    public ComicFileSystemServiceTests()
    {
        Directory.CreateDirectory(FolderPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_baseFolder))
            Directory.Delete(_baseFolder, true);
    }

    [Fact]
    public async Task Image_Returned()
    {
        // Arrange
        var date = new DateOnly(2023, 01, 01);
        var dataArray = $"test".Select(c => (byte)c).ToArray();

        var imagePath = $"{FolderPath}{date:yyyy_MM_dd}.png";
        var fs = File.Create(imagePath);
        await fs.WriteAsync(dataArray);
        await fs.FlushAsync();
        fs.Close();
        fs.Dispose();

        ComicFileSystemService service = new();

        // Act
        var stream = await service.GetImageAsync(date, SourceType.Acomics);

        // Assert
        Assert.Equal(dataArray.Length, stream.Length);
    }

    [Fact]
    public async Task Image_Saved()
    {
        // Arrange
        var date = new DateOnly(2023, 01, 02);
        var dataArray = $"test".Select(c => (byte)c).ToArray();

        using var ms = new MemoryStream(dataArray);

        ComicFileSystemService service = new();

        // Act
        await service.SaveImageAsync(ms, date, SourceType.Acomics);

        // Assert
        Assert.True(File.Exists($"{FolderPath}{date:yyyy_MM_dd}.png"));
    }
}
