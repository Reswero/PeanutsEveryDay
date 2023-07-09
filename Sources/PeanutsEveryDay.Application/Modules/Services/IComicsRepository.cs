using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Application.Modules.Services;

public interface IComicsRepository
{
    public Task AddAsync(Comic comic, CancellationToken cancellationToken = default);
}
