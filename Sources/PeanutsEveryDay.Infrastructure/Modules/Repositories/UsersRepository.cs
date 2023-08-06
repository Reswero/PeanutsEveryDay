using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PeanutsEveryDay.Infrastructure.Modules.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly PeanutsContext _db;

    public UsersRepository(PeanutsContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _db.Users.Include(u => u.Progress)
            .Include(u => u.Settings)
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<List<User>> GetByFilterAsync(Expression<Func<User, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var users = await _db.Users.Include(u => u.Progress)
            .Include(u => u.Settings)
            .Where(filter)
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<User?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.Include(u => u.Progress)
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
