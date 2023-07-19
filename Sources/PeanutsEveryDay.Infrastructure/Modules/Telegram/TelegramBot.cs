using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram;

public class TelegramBot : IUpdateHandler
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TelegramBot> _logger;

    private readonly TelegramBotClient _bot;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
    };

    public TelegramBot(string token, IServiceProvider services)
    {
        _services = services;
        _logger = services.GetRequiredService<ILogger<TelegramBot>>();

        _bot = new(token);
        _bot.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions);

        var comicsService = services.GetRequiredService<IComicsService>();
        NextComic.Init(comicsService);
        ComicByDate.Init(comicsService);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not null)
                await HandleMessageAsync(update.Message, cancellationToken);
            else if (update.CallbackQuery is not null)
                await HandleCallbackAsync(update.CallbackQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Error}", ex.Message);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Message handling error. {Error}", exception.Message);
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
        {
            return;
        }

        using var scope = _services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = message.From!.Id;
        var user = await repository.GetAsync(chatId, cancellationToken);

        if (user is null)
        {
            user = Dom.User.Create(chatId, message.From.FirstName, message.From.Username);
            await repository.AddAsync(user, cancellationToken);
        }

        if (message.Text == CommandDictionary.Start ||
            message.Text == CommandDictionary.SetMenu)
        {
            await KeyboardMenu.SendAsync(_bot, user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.NextComic)
        {
            await NextComic.SendAsync(_bot, user, cancellationToken);
            await repository.UpdateAsync(user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.Menu)
        {
            await MainMenu.SendAsync(_bot, user, cancellationToken);
        }
        else if (message.Text.StartsWith(CommandDictionary.ComicByDate))
        {
            string textDate = message.Text[CommandDictionary.ComicByDate.Length..];
            var parsed = DateOnly.TryParse(textDate, out var date);

            if (parsed is true)
            {
                await ComicByDate.SendAsync(_bot, date, user, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                await _bot.SendTextMessageAsync(user.Id, AnswerDictionary.WrongDateFormat,
                    cancellationToken: cancellationToken);
            }
        }
        else if (message.Text.StartsWith(CommandDictionary.SetDate))
        {
            string textDate = message.Text[CommandDictionary.SetDate.Length..];
            var parsed = DateOnly.TryParse(textDate, out var date);

            if (parsed is true)
            {
                await SetDate.ExecuteAsync(_bot, date, user, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                await _bot.SendTextMessageAsync(user.Id, AnswerDictionary.WrongDateFormat,
                    cancellationToken: cancellationToken);
            }
        }
    }

    private async Task HandleCallbackAsync(CallbackQuery callback, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = callback.From.Id;
        var user = (await repository.GetAsync(chatId, cancellationToken))!;

        if (callback.Data is null)
            return;

        if (callback.Data.StartsWith(CallbackDictionary.SourcePrefix))
        {
            CallbackHelper.ChangeSources(callback.Data, user!.Settings);
            await repository.UpdateAsync(user, cancellationToken);
            callback.Data = CallbackDictionary.Sources;
        }
        else if (callback.Data.StartsWith(CallbackDictionary.PeriodPrefix))
        {
            CallbackHelper.ChangePeriod(callback.Data, user!.Settings);
            await repository.UpdateAsync(user, cancellationToken);
            callback.Data = CallbackDictionary.Period;
        }

        var (template, inlineKeyboard) = CallbackHelper.GetTemplateWithKeyboardMarkup(callback.Data, user);

        int messageId = callback.Message!.MessageId;
        if (template is not null && inlineKeyboard is not null)
        {
            await _bot.EditMessageTextAsync(user.Id, messageId, template, replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        else
        {
            await _bot.DeleteMessageAsync(user.Id, messageId, cancellationToken);
        }
    }
}
