using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Application.Modules.Services;

/// <summary>
/// Interaction with comic images in file system
/// </summary>
public interface IComicFileSystemService
{
    /// <summary>
    /// Gets image by date and source type
    /// </summary>
    /// <param name="date">Comic date</param>
    /// <param name="source">Comic source type</param>
    /// <returns>Image stream</returns>
    public Task<Stream> GetImage(DateOnly date, SourceType source, CancellationToken cancellationToken = default);
    /// <summary>
    /// Saves image to specified folder by source type
    /// </summary>
    /// <param name="stream">Image stream</param>
    /// <param name="date">Comic date</param>
    /// <param name="source">Comic source type</param>
    public Task SaveImage(Stream stream, DateOnly date, SourceType source, CancellationToken cancellationToken = default);
}
