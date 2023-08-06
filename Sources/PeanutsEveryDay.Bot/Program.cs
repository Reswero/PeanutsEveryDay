using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure;
using PeanutsEveryDay.Infrastructure.Modules.Telegram;

namespace PeanutsEveryDay.Bot;

internal class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .RegisterServices()
            .BuildServiceProvider();

        serviceProvider.InitializeDatabase();

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var token = configuration["TelegramAPI:Token"]!;

        TelegramBot bot = new(token, serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var loader = scope.ServiceProvider.GetRequiredService<IComicsLoaderService>();
        await loader.LoadAsync(TimeSpan.FromMinutes(3));

        Console.ReadLine();
    }
}