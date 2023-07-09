using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure;

namespace PeanutsEveryDay.Bot;

internal class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .RegisterServices()
            .BuildServiceProvider();

        serviceProvider.InitializeDatabase();

        using var scope = serviceProvider.CreateScope();
        var loader = scope.ServiceProvider.GetRequiredService<IComicsLoaderService>();
        await loader.LoadAsync(TimeSpan.FromMinutes(3));
    }
}