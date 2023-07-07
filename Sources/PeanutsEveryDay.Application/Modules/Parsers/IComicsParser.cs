using PeanutsEveryDay.Application.Modules.Parsers.Models;

namespace PeanutsEveryDay.Application.Modules.Parsers;

/// <summary>
/// Peanuts comics parser
/// </summary>
public interface IComicsParser
{
    /// <summary>
    /// Parsing Peanuts comics
    /// </summary>
    public IAsyncEnumerable<ParsedComic> ParseAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Parsing Peanuts Begins comics
    /// </summary>
    public IAsyncEnumerable<ParsedComic> ParseBeginsAsync(CancellationToken cancellationToken = default);
}
