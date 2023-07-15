using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;

namespace PeanutsEveryDay.Infrastructure.Modules.Services;

public class ComicFileSystemService : IComicFileSystemService
{
    public async Task<Stream> GetImageAsync(DateOnly date, SourceType source, CancellationToken cancellationToken = default)
    {
        string folderPath = GetFolderPath(source);
        string imagePath = $"{folderPath}/{date:yyyy_MM_dd}.png";

        var bytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
        return new MemoryStream(bytes);
    }

    public async Task SaveImageAsync(Stream stream, DateOnly date, SourceType source, CancellationToken cancellationToken = default)
    {
        string folderPath = GetFolderPath(source);
        Directory.CreateDirectory(folderPath);

        using var fs = File.Create($"{folderPath}/{date:yyyy_MM_dd}.png");
        await stream.CopyToAsync(fs, cancellationToken);
        await fs.FlushAsync(cancellationToken);
    }

    private string GetFolderPath(SourceType source)
    {
        string path = $"comics/";

        switch (source)
        {
            case SourceType.Acomics:
                path += "acomics/peanuts/";
                break;
            case SourceType.AcomicsBegins:
                path += "acomics/peanutsbegins/";
                break;
            case SourceType.Gocomics:
                path += "gocomics/peanuts/";
                break;
            case SourceType.GocomicsBegins:
                path += "gocomics/peanutsbegins/";
                break;
        }

        return path;
    }
}
