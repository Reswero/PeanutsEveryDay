using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Application.Modules.Repositories;

public interface IUsersRepository
{
    public Task AddAsync(User user, CancellationToken cancellation = default);
    public Task<User?> GetAsync(long id, CancellationToken cancellation = default);
    public Task UpdateAsync(User user, CancellationToken cancellation = default);
}
