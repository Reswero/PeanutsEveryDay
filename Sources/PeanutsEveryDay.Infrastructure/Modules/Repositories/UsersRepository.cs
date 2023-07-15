using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Data;
using PeanutsEveryDay.Domain.Models;

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
        Data.Models.User userDb = new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Username = user.Username,
            Progress = new() { UserId = user.Id },
            Settings = new() { UserId = user.Id }
        };

        await _db.Users.AddAsync(userDb, cancellation);
        await _db.SaveChangesAsync(cancellation);
    }

    public async Task<User?> GetAsync(long id, CancellationToken cancellation = default)
    {
        var userDb = await _db.Users.Include(u => u.Progress)
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Id == id, cancellation);

        if (userDb is null)
        {
            return null;
        }

        return new()
        {
            Id = userDb.Id,
            FirstName = userDb.FirstName,
            Username = userDb.Username,
            Progress = new()
            {
                UserId = userDb.Id,
                TotalComicsWatched = userDb.Progress!.TotalComicsWatched
            },
            Settings = new()
            {
                UserId = userDb.Id
            }
        };
    }

    public async Task UpdateAsync(User user, CancellationToken cancellation = default)
    {
        Data.Models.UserProgress progressDb = new()
        {
            UserId = user.Id,
            TotalComicsWatched = user.Progress.TotalComicsWatched
        };
        Data.Models.UserSettings settingsDb = new()
        {
            UserId = user.Id
        };
        Data.Models.User userDb = new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Username = user.Username,
            Progress = progressDb,
            Settings = settingsDb
        };

        _db.Users.Update(userDb);
        await _db.SaveChangesAsync(cancellation);
    }
}
