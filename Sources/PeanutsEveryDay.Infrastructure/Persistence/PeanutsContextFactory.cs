using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PeanutsEveryDay.Infrastructure.Persistence;

public class PeanutsContextFactory : IDesignTimeDbContextFactory<PeanutsContext>
{
    public PeanutsContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PeanutsContext>();
        builder.UseNpgsql();

        return new PeanutsContext(builder.Options);
    }
}
