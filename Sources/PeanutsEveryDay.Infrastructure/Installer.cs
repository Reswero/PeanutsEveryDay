using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure.Modules.Converters;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Persistence;

namespace PeanutsEveryDay.Infrastructure;

public static class Installer
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("settings.json")
            .Build();

        services.AddSingleton(configuration);

        services.AddDbContext<PeanutsContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddLogging(cfg =>
        {
            cfg.ClearProviders();
            cfg.SetMinimumLevel(LogLevel.Information);
            cfg.AddConsole();
            cfg.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
        });

        services.AddScoped<IComicsParser, AcomicsParser>();
        services.AddScoped<IComicsParser, GocomicsParser>();

        services.AddScoped<IComicImageConverter, ComicImageConverter>();
        services.AddScoped<IComicFileSystemService, ComicFileSystemService>();

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IComicsRepository, ComicsRepository>();
        services.AddScoped<IParserStateRepository, ParserStateRepository>();

        services.AddScoped<IComicsService, ComicsService>();

        services.AddSingleton<IComicsLoaderService, ComicsLoaderService>();
        services.AddSingleton<TimeComicsSenderService>();
        services.AddSingleton<TelegramBot>(provider =>
        {
            var token = configuration["TelegramAPI:Token"]!;
            return new(token, provider);
        });
        services.AddSingleton<MessagesSenderService>(provider =>
        {
            var botClient = provider.GetRequiredService<TelegramBot>().Client;
            return new(botClient);
        });

        return services;
    }

    public static void InitializeDatabase(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<PeanutsContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public static void InitializeTelegramBot(this IServiceProvider provider)
    {
        _ = provider.GetRequiredService<TelegramBot>();
        provider.InitializeHandlers();
        provider.InitializeCommands();

        var comicsSenderService = provider.GetRequiredService<TimeComicsSenderService>();
        comicsSenderService.Start();
    }

    private static void InitializeHandlers(this IServiceProvider provider)
    {
        var senderService = provider.GetRequiredService<MessagesSenderService>();

        MessageHandler.Init(provider, senderService);
        CallbackHandler.Init(provider, senderService);
    }

    private static void InitializeCommands(this IServiceProvider provider)
    {
        var comicsService = provider.GetRequiredService<IComicsService>();
        var senderService = provider.GetRequiredService<MessagesSenderService>();

        MainMenu.Init(senderService);
        KeyboardMenu.Init(senderService);
        NextComic.Init(comicsService, senderService);
        ComicByDate.Init(comicsService, senderService);
    }
}
