using PeanutsEveryDay.Domain.Models;
using System.Linq.Expressions;

namespace PeanutsEveryDay.Application.Modules.Repositories;

public interface IUsersRepository
{
    public Task AddAsync(User user, CancellationToken cancellationToken = default);
    public Task<User?> GetAsync(long id, CancellationToken cancellationToken = default);
    public Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<List<User>> GetByFilterAsync(Expression<Func<User, bool>> filter,
        CancellationToken cancellationToken = default);
}
