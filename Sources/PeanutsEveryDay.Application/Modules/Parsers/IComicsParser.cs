using PeanutsEveryDay.Application.Modules.Parsers.Models;
using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Application.Modules.Parsers;

/// <summary>
/// Peanuts comics parser
/// </summary>
public interface IComicsParser
{
    /// <summary>
    /// Sets parser state
    /// </summary>
    /// <param name="state">Parser state</param>
    public void SetState(ParserState state);
    /// <summary>
    /// Parsing Peanuts comics
    /// </summary>
    public IAsyncEnumerable<ParsedComic> ParseAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Parsing Peanuts Begins comics
    /// </summary>
    public IAsyncEnumerable<ParsedComic> ParseBeginsAsync(CancellationToken cancellationToken = default);
}
