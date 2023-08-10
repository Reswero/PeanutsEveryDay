using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure;
using PeanutsEveryDay.Infrastructure.Modules.Telegram;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

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

        MessagesSenderService messagesSenderService = new(bot.Client);
        var comicsService = serviceProvider.GetRequiredService<IComicsService>();

        InitHandlers(serviceProvider, messagesSenderService);
        InitCommands(messagesSenderService, comicsService);

        var comicsSenderService = serviceProvider.GetRequiredService<TimeComicsSenderService>();
        comicsSenderService.Start();

        var loader = serviceProvider.GetRequiredService<IComicsLoaderService>();
        await loader.LoadAsync(TimeSpan.FromMinutes(10));

        Console.ReadLine();
    }

    private static void InitHandlers(IServiceProvider serviceProvider, MessagesSenderService senderService)
    {
        MessageHandler.Init(serviceProvider, senderService);
        CallbackHandler.Init(serviceProvider, senderService);
    }

    private static void InitCommands(MessagesSenderService senderService, IComicsService comicsService)
    {
        MainMenu.Init(senderService);
        KeyboardMenu.Init(senderService);
        NextComic.Init(comicsService, senderService);
        ComicByDate.Init(comicsService, senderService);
    }
}