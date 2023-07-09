using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Application.Modules.Repositories;

public interface IParserStateRepository
{
    public Task<ParserState> GetAsync(CancellationToken cancellationToken = default);
    public Task AddOrUpdateAsync(ParserState state, CancellationToken cancellationToken = default);
}
