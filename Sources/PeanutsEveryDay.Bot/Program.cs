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
        serviceProvider.InitializeTelegramBot();

        var loader = serviceProvider.GetRequiredService<IComicsLoaderService>();
        await loader.StartLoadingAsync();

        Console.ReadLine();
    }
}