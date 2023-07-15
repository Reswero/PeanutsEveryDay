using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        long chatId = message.Chat.Id;
        var user = await repository.GetAsync(chatId, cancellationToken);

        if (user is null)
        {
            user = Dom.User.Create(chatId, message.Chat.FirstName!, message.Chat.Username);
            await repository.AddAsync(user, cancellationToken);
        }

        if (message.Text == "/start")
        {
            await SendStartMenuAsync(user, cancellationToken);
        }
        else if (message.Text == "Следующий ➡️")
        {
            await SendNextComicAsync(user, cancellationToken);
            await repository.UpdateAsync(user, cancellationToken);
        }
    }

    private async Task SendStartMenuAsync(Dom.User user, CancellationToken cancellationToken)
    {
        ReplyKeyboardMarkup replyKeyboard = new(new[]
        {
            new KeyboardButton("Следующий ➡️")
        });

        replyKeyboard.ResizeKeyboard = true;

        await _bot.SendTextMessageAsync(user.Id, "Привет! :)", replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken);
    }

    private async Task SendNextComicAsync(Dom.User user, CancellationToken cancellationToken)
    {
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
}
