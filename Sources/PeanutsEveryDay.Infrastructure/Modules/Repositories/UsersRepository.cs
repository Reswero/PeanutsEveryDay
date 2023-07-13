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
        Data.Models.User userDb = new() { Id = user.Id, FirstName = user.FirstName, Username = user.Username };
        Data.Models.UserSettings settingsDb = new() { User = userDb };
        Data.Models.UserProgress progressDb = new() { User = userDb };

        await _db.Users.AddAsync(userDb, cancellation);
        await _db.UsersSettings.AddAsync(settingsDb, cancellation);
        await _db.UsersProgress.AddAsync(progressDb, cancellation);
        await _db.SaveChangesAsync(cancellation);
    }

    public async Task<User> GetAsync(long id, CancellationToken cancellation = default)
    {
        var userDb = await _db.Users.FirstAsync(u => u.Id == id, cancellation);

        return new()
        {
            Id = userDb.Id,
            FirstName = userDb.FirstName,
            Username = userDb.Username,
        };
    }

    public async Task UpdateAsync(User user, CancellationToken cancellation = default)
    {
        Data.Models.User userDb = new() { Id = user.Id, FirstName = user.FirstName, Username = user.Username };

        _db.Users.Update(userDb);
        await _db.SaveChangesAsync(cancellation);
    }
}
