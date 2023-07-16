using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using Telegram.Bot;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class MainMenu
{
    public static async Task SendAsync(ITelegramBotClient bot, User user, CancellationToken cancellationToken)
    {
        var (template, inlineKeyboard) =
            CallbackHelper.GetTemplateWithKeyboardMarkup(CallbackDictionary.MainMenu, user);

        await bot.SendTextMessageAsync(user.Id, template!, replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}
