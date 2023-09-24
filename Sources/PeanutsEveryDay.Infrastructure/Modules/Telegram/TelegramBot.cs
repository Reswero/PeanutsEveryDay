using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram;

public class TelegramBot : IUpdateHandler
{
    private readonly ILogger<TelegramBot> _logger;

    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
    };

    public TelegramBot(string token, IServiceProvider services)
    {
        _logger = services.GetRequiredService<ILogger<TelegramBot>>();

        Client = new TelegramBotClient(token);
        Client.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions);
    }

    public ITelegramBotClient Client { get; }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not null)
            {
                await MessageHandler.HandleAsync(update.Message, cancellationToken);
            }
            else if (update.CallbackQuery is not null)
            {
                await CallbackHandler.HandleAsync(update.CallbackQuery, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            if (update.Message is not null && update.Message.Text is not null)
            {
                _logger.LogError(ex, "An exception occurred while processing a user message. Message: '{Text}'. {Error}",
                    update.Message.Text, ex.Message);
            }
            else if (update.CallbackQuery is not null && update.CallbackQuery.Data is not null)
            {
                _logger.LogError(ex, "An exception occurred while processing a user callback. Callback: '{Data}'. {Error}",
                    update.CallbackQuery.Data, ex.Message);
            }
            else
            {
                _logger.LogError(ex, "An exception occurred while processing a user request. {Error}", ex.Message);
            }
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogCritical(exception, "Bot crashed. {Error}", exception.Message);
        return Task.CompletedTask;
    }
}
