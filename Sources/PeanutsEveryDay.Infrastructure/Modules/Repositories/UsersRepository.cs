using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;

namespace PeanutsEveryDay.Infrastructure.Modules.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly PeanutsContext _db;

    public UsersRepository(PeanutsContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user, CancellationToken cancellation = default)
    {
        await _db.Users.AddAsync(user, cancellation);
        await _db.SaveChangesAsync(cancellation);
    }

    public async Task<User?> GetAsync(long id, CancellationToken cancellation = default)
    {
        var user = await _db.Users.Include(u => u.Progress)
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Id == id, cancellation);

        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellation = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellation);
    }
}
