using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class KeyboardMenu
{
    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(User user, CommandDictionary commandDictionary,
        AnswerDictionary answerDictionary, CancellationToken cancellationToken)
    {
        var replyKeyboard = KeyboardHelper.CreateReplyKeyboard(commandDictionary);
        _senderService.EnqueueMessage(new TextMessage(user.Id, answerDictionary.KeyboardInstalled, replyKeyboard));
    }
}
