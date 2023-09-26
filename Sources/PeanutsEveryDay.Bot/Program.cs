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
        serviceProvider.InitializeMetrics();
        serviceProvider.InitializeTelegramBot();

        var loader = serviceProvider.GetRequiredService<IComicsLoaderService>();
        _ = Task.Run(async () => await loader.StartLoadingAsync());

        while (true)
        {
            var command = Console.ReadLine() ?? string.Empty;

            if (command == "/quit")
            {
                await ConsoleCommands.ExecuteQuitCommand();
            }
            else if (command.StartsWith("/metrics"))
            {
                await ConsoleCommands.ExecuteMetricsCommand(command);
            }
        }
    }
}