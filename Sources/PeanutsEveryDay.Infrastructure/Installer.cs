using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Converters;
using PeanutsEveryDay.Application.Modules.Parsers;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure.Modules.Converters;
using PeanutsEveryDay.Infrastructure.Modules.Metrics;
using PeanutsEveryDay.Infrastructure.Modules.Parsers;
using PeanutsEveryDay.Infrastructure.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.En;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Ru;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using PeanutsEveryDay.Infrastructure.Persistence;
using Serilog;
using Serilog.Events;
namespace PeanutsEveryDay.Infrastructure;

public static class Installer
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromSeconds(5))
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("settings.json")
            .Build();

        services.AddSingleton(configuration);

        services.AddDbContext<PeanutsContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
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
            var logger = provider.GetRequiredService<ILogger<MessagesSenderService>>();
            var botClient = provider.GetRequiredService<TelegramBot>().Client;
            return new(logger, botClient);
        });

        services.AddCommandDictionaries();
        services.AddAnswerDictionaries();
        services.AddCallbackDictionaries();

        return services;
    }

    public static void InitializeDatabase(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<PeanutsContext>();

        db.Database.Migrate();
    }

    public static void InitializeTelegramBot(this IServiceProvider provider)
    {
        var ru = provider.GetRequiredService<AnswerDictionaryResolver>().Invoke(LanguageCode.Ru);
        var en = provider.GetRequiredService<AnswerDictionaryResolver>().Invoke(LanguageCode.En);

        Dictionary<string, AnswerDictionary> dictionaries = new();
        dictionaries["ru"] = ru;
        dictionaries["en"] = en;

        provider.GetRequiredService<TelegramBot>().Init(dictionaries);
        provider.InitializeHandlers();
        provider.InitializeCommands();

        var comicsSenderService = provider.GetRequiredService<TimeComicsSenderService>();
        comicsSenderService.Start();
    }

    public static void InitializeMetrics(this IServiceProvider provider)
    {
        SimpleMetricsService.Init(provider);
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

        ComicByDate.Init(comicsService, senderService);
        CommandMenu.Init(senderService);
        HelpInfo.Init(senderService);
        KeyboardMenu.Init(senderService);
        MainMenu.Init(senderService);
        NextComic.Init(comicsService, senderService);
        Start.Init(senderService);
        StopSending.Init(senderService);
    }

    private static void AddCommandDictionaries(this IServiceCollection services)
    {
        services.AddSingleton<RuCommandDictionary>();
        services.AddSingleton<EnCommandDictionary>();

        services.AddTransient<CommandDictionaryResolver>(provider => language =>
        {
            return language switch
            {
                LanguageCode.Ru => provider.GetRequiredService<RuCommandDictionary>(),
                LanguageCode.En => provider.GetRequiredService<EnCommandDictionary>(),
                _ => throw new NotImplementedException()
            };
        });
    }

    private static void AddAnswerDictionaries(this IServiceCollection services)
    {
        services.AddSingleton<RuAnswerDictionary>();
        services.AddSingleton<EnAnswerDictionary>();

        services.AddTransient<AnswerDictionaryResolver>(provider => language =>
        {
            return language switch
            {
                LanguageCode.Ru => provider.GetRequiredService<RuAnswerDictionary>(),
                LanguageCode.En => provider.GetRequiredService<EnAnswerDictionary>(),
                _ => throw new NotImplementedException()
            };
        });
    }

    private static void AddCallbackDictionaries(this IServiceCollection services)
    {
        services.AddSingleton<RuCallbackDictionary>();
        services.AddSingleton<EnCallbackDictionary>();

        services.AddTransient<CallbackDictionaryResolver>(provider => language =>
        {
            return language switch
            {
                LanguageCode.Ru => provider.GetRequiredService<RuCallbackDictionary>(),
                LanguageCode.En => provider.GetRequiredService<EnCallbackDictionary>(),
                _ => throw new NotImplementedException()
            };
        });
    }
}
