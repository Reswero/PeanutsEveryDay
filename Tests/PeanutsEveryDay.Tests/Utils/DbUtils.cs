using Microsoft.EntityFrameworkCore;
using PeanutsEveryDay.Infrastructure.Persistence;

namespace PeanutsEveryDay.Tests.Utils;

public static class DbUtils
{
    public static PeanutsContext CreateContext(int? seed = null)
    {
        seed ??= Random.Shared.Next(1, 1_000_000);

        DbContextOptions<PeanutsContext> options =
            new DbContextOptionsBuilder<PeanutsContext>()
                .UseNpgsql($"Host=localhost;Port=5432;Database=peanuts_tests_{seed};Username=postgres;Password=root")
                .Options;

        return new PeanutsContext(options);
    }

    public static void CreateDatabase(PeanutsContext context)
    {
        context.Database.EnsureCreated();
    }

    public static void DropDatabase(PeanutsContext context)
    {
        context.Database.EnsureDeleted();
    }

    public static void ClearTracker(PeanutsContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }
}
