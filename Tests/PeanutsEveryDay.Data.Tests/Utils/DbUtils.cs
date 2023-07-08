using Microsoft.EntityFrameworkCore;

namespace PeanutsEveryDay.Data.Tests.Utils;

public static class DbUtils
{
    public static PeanutsContext CreateContext(int seed)
    {
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
}
