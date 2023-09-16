using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class MainMenu
{
    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(User user, CallbackDictionary dictionary, CancellationToken cancellationToken)
    {
        var (template, inlineKeyboard) =
            CallbackHelper.GetTemplateWithKeyboardMarkup(CallbackKey.MainMenu, user, dictionary);

        _senderService.EnqueueMessage(new TextMessage(user.Id, template, inlineKeyboard));
    }
}
