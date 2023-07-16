using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class SettingsMenu
{
    public static async Task SendAsync(ITelegramBotClient bot, User user, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = CallbackHelper.GetKeyboardMarkup(CallbackDictionary.MainMenu, user.Settings)!;

        await bot.SendTextMessageAsync(user.Id, CommandDictionary.Settings, replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}
