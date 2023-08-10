using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class KeyboardMenu
{
    private static readonly ReplyKeyboardMarkup _replyKeyboard = new(new[]
    {
            new KeyboardButton(CommandDictionary.NextComic),
            new KeyboardButton(CommandDictionary.Menu)
    })
    {
        ResizeKeyboard = true
    };

    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(User user, CancellationToken cancellationToken)
    {
        _senderService.EnqueueMessage(new TextMessage(user.Id, AnswerDictionary.Greetings, _replyKeyboard));
    }
}
