namespace PeanutsEveryDay.Application.Modules.Converters;

/// <summary>
/// Converts comic image into variety of a representations
/// </summary>
public interface IComicImageConverter
{
    /// <summary>
    /// Converts strip comic to square. Writes result in a passed stream
    /// </summary>
    /// <param name="stream">Image stream</param>
    public Task ConvertFromStripToSquareAsync(Stream stream, CancellationToken cancellationToken = default);
}
