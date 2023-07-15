using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Parsers.Models;

namespace PeanutsEveryDay.Application.Modules.Services;

public interface IComicsService
{
    public Task<ParsedComic?> GetComicAsync(DateOnly date, SourceType sources, CancellationToken cancellationToken = default);
}
