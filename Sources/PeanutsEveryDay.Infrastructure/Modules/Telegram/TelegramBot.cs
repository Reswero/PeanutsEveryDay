using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram;

public class TelegramBot : IUpdateHandler
{
    private readonly IServiceProvider _services;
    private readonly IComicsService _comicsService;
    private readonly ILogger<TelegramBot> _logger;

    private readonly TelegramBotClient _bot;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
    };

    public TelegramBot(string apiKey, IServiceProvider services)
    {
        _services = services;
        _comicsService = services.GetRequiredService<IComicsService>();
        _logger = services.GetRequiredService<ILogger<TelegramBot>>();

        _bot = new(apiKey);
        _bot.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not null)
            await HandleMessageAsync(update.Message, cancellationToken);
        else if (update.CallbackQuery is not null)
            await HandleCallbackAsync(update.CallbackQuery, cancellationToken);
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message handling error. {Error}", exception.Message);
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = message.From!.Id;
        var user = await repository.GetAsync(chatId, cancellationToken);

        if (user is null)
        {
            user = Dom.User.Create(chatId, message.From.FirstName, message.From.Username);
            await repository.AddAsync(user, cancellationToken);
        }

        if (message.Text == "/start")
        {
            await SendStartMenuAsync(user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.NextComic)
        {
            await SendNextComicAsync(user, cancellationToken);
            await repository.UpdateAsync(user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.Settings)
        {
            await SendSettingsMenuAsync(user, cancellationToken);
        }
    }

    private async Task SendStartMenuAsync(Dom.User user, CancellationToken cancellationToken)
    {
        ReplyKeyboardMarkup replyKeyboard = new(new[]
        {
            new KeyboardButton(CommandDictionary.NextComic),
            new KeyboardButton(CommandDictionary.Settings)
        });

        replyKeyboard.ResizeKeyboard = true;

        await _bot.SendTextMessageAsync(user.Id, "Привет! :)", replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task SendNextComicAsync(Dom.User user, CancellationToken cancellationToken)
    {
        if (user.Settings.Sources == SourceType.None)
        {
            await _bot.SendTextMessageAsync(user.Id, "Должен быть выбран хотя бы один источник с комиксами!",
                cancellationToken: cancellationToken);
            return;
        }

        var nextDate = user.Progress.LastWatchedComicDate.AddDays(1);
        var comic = await _comicsService.GetComicAsync(nextDate, user.Settings.Sources, cancellationToken);

        if (comic is null)
        {
            await _bot.SendTextMessageAsync(user.Id, "Комиксы закончились :(", cancellationToken: cancellationToken);
            return;
        }

        string text = $"[{comic.PublicationDate:dd MMMM yyyy}]({comic.Url})";
        InputFileStream inputFile = new(comic.ImageStream, comic.PublicationDate.ToShortDateString());

        await _bot.SendPhotoAsync(user.Id, inputFile, cancellationToken: cancellationToken);
        await _bot.SendTextMessageAsync(user.Id, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        user.Progress.IncreaseDate();
    }

    private async Task SendSettingsMenuAsync(Dom.User user, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Источники", CallbackDictionary.Sources) },
            new[] { InlineKeyboardButton.WithCallbackData("Скрыть", CallbackDictionary.Hide) }
        });

        await _bot.SendTextMessageAsync(user.Id, CommandDictionary.Settings, replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = callback.From.Id;
        var user = await repository.GetAsync(chatId, cancellationToken);

        if (callback.Data is null)
            return;

        if (callback.Data.StartsWith(CallbackDictionary.SourcePrefix))
        {
            ChangeSources(user!.Settings, callback.Data);
            await repository.UpdateAsync(user, cancellationToken);
            callback.Data = CallbackDictionary.Sources;
        }

        var inlineKeyboard = GetKeyboardMarkup(user!.Settings, callback.Data);

        int messageId = callback.Message!.MessageId;
        if (inlineKeyboard is not null)
        {
            await _bot.EditMessageReplyMarkupAsync(user.Id, messageId, inlineKeyboard, cancellationToken);
        }
        else
        {
            await _bot.DeleteMessageAsync(user.Id, messageId, cancellationToken);
        }
    }

    private void ChangeSources(Dom.UserSettings settings, string callback)
    {
        switch (callback)
        {
            case CallbackDictionary.AcomicsSource:
                settings.InverseSource(SourceType.Acomics);
                break;
            case CallbackDictionary.AcomicsBeginsSource:
                settings.InverseSource(SourceType.AcomicsBegins);
                break;
            case CallbackDictionary.GocomicsSource:
                settings.InverseSource(SourceType.Gocomics);
                break;
            case CallbackDictionary.GocomicsBeginsSource:
                settings.InverseSource(SourceType.GocomicsBegins);
                break;
        }
    }

    private InlineKeyboardMarkup? GetKeyboardMarkup(Dom.UserSettings settings, string callback)
    {
        InlineKeyboardMarkup? keyboardMarkup = null;
        if (callback == CallbackDictionary.Sources)
        {
            string? acm = settings.Sources.HasFlag(SourceType.Acomics) ? " ✅" : null;
            string? acmB = settings.Sources.HasFlag(SourceType.AcomicsBegins) ? " ✅" : null;
            string? gcm = settings.Sources.HasFlag(SourceType.Gocomics) ? " ✅" : null;
            string? gcmB = settings.Sources.HasFlag(SourceType.GocomicsBegins) ? " ✅" : null;

            keyboardMarkup = new(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Acomics (RU)" + acm, CallbackDictionary.AcomicsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Acomics Begins (RU)" + acmB, CallbackDictionary.AcomicsBeginsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Gocomics (EN)" + gcm, CallbackDictionary.GocomicsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Gocomics Begins (EN)" + gcmB, CallbackDictionary.GocomicsBeginsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.BackFromSources) }
            });
        }
        else if (callback == CallbackDictionary.BackFromSources)
        {
            keyboardMarkup = new(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Источники", CallbackDictionary.Sources) },
                new[] { InlineKeyboardButton.WithCallbackData("Скрыть", CallbackDictionary.Hide) }
            });
        }

        return keyboardMarkup;
    }
}
