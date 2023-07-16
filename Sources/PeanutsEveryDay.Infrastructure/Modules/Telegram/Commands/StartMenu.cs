using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class StartMenu
{
    private static readonly ReplyKeyboardMarkup _replyKeyboard = new(new[]
    {
            new KeyboardButton(CommandDictionary.NextComic),
            new KeyboardButton(CommandDictionary.Menu)
    })
    {
        ResizeKeyboard = true
    };

    public static async Task SendAsync(ITelegramBotClient bot, User user, CancellationToken cancellationToken)
    {
        await bot.SendTextMessageAsync(user.Id, "Привет! :)", replyMarkup: _replyKeyboard,
            cancellationToken: cancellationToken);
    }
}
