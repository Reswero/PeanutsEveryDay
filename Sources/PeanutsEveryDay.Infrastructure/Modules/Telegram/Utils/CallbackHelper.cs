using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

public static class CallbackHelper
{
    public static InlineKeyboardMarkup? GetKeyboardMarkup(string callback, UserSettings settings)
    {
        InlineKeyboardMarkup? keyboardMarkup = null;
        if (callback == CallbackDictionary.MainMenu)
        {
            keyboardMarkup = new(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Настройки", CallbackDictionary.Settings) },
                new[] { InlineKeyboardButton.WithCallbackData("Скрыть", CallbackDictionary.Hide) }
            });
        }
        else if (callback == CallbackDictionary.Settings)
        {
            keyboardMarkup = new(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Источники", CallbackDictionary.Sources) },
                new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.MainMenu) }
            });
        }
        else if (callback == CallbackDictionary.Sources)
        {
            string? acm = settings.Sources.HasFlag(SourceType.Acomics) ? " ✅" : null;
            string? acmB = settings.Sources.HasFlag(SourceType.AcomicsBegins) ? " ✅" : null;
            string? gcm = settings.Sources.HasFlag(SourceType.Gocomics) ? " ✅" : null;
            string? gcmB = settings.Sources.HasFlag(SourceType.GocomicsBegins) ? " ✅" : null;

            keyboardMarkup = new(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Acomics (RU)" + acm, CallbackDictionary.AcomicsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Acomics Begins (RU)" + acmB, CallbackDictionary.AcomicsBeginsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Gocomics (EN)" + gcm, CallbackDictionary.GocomicsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Gocomics Begins (EN)" + gcmB, CallbackDictionary.GocomicsBeginsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.Settings) }
            });
        }

        return keyboardMarkup;
    }

    public static void ChangeSources(string callback, UserSettings settings)
    {
        switch (callback)
        {
            case CallbackDictionary.AcomicsSource:
                settings.InverseSource(SourceType.Acomics);
                break;
            case CallbackDictionary.AcomicsBeginsSource:
                settings.InverseSource(SourceType.AcomicsBegins);
                break;
            case CallbackDictionary.GocomicsSource:
                settings.InverseSource(SourceType.Gocomics);
                break;
            case CallbackDictionary.GocomicsBeginsSource:
                settings.InverseSource(SourceType.GocomicsBegins);
                break;
        }
    }
}
