using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

public static class KeyboardHelper
{
    public static ReplyKeyboardMarkup CreateReplyKeyboard(CommandDictionary dictionary)
    {
        ReplyKeyboardMarkup replyKeyboard = new(new[]
        {
            new KeyboardButton(dictionary.NextComic),
            new KeyboardButton(dictionary.Menu)
        })
        {
            ResizeKeyboard = true
        };

        return replyKeyboard;
    }
}
