using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram;

public class TelegramBot : IUpdateHandler
{
    private readonly IServiceProvider _services;

    private readonly TelegramBotClient _bot;
    private readonly ReceiverOptions _receiverOptions = new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
    };

    public TelegramBot(string apiKey, IServiceProvider services)
    {
        _services = services;

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
        throw new NotImplementedException();
    }

    private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;
        await _bot.SendTextMessageAsync(chatId, message.Text!);
    } 
}
