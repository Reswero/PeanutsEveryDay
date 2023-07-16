using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

public static class CallbackHelper
{
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

    public static (string?, InlineKeyboardMarkup?) GetTemplateWithKeyboardMarkup(string callback, User user)
    {
        return callback switch
        {
            CallbackDictionary.MainMenu => GetMainMenu(),
            CallbackDictionary.Progress => GetProgressMenu(user.Progress),
            CallbackDictionary.Settings => GetSettingsMenu(),
            CallbackDictionary.Sources => GetSourcesMenu(user.Settings),
            _ => (null, null),
        };
    }

    private static (string, InlineKeyboardMarkup) GetMainMenu()
    {
        string template = "Меню";
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Прогресс", CallbackDictionary.Progress) },
            new[] { InlineKeyboardButton.WithCallbackData("Настройки", CallbackDictionary.Settings) },
            new[] { InlineKeyboardButton.WithCallbackData("Скрыть", CallbackDictionary.Hide) }
        });
        
        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetProgressMenu(UserProgress progress)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var startDate = new DateOnly(1950, 10, 01);

        var watched = progress.LastWatchedComicDate.DayNumber - startDate.DayNumber;
        var totalComics = currentDate.DayNumber - startDate.DayNumber;

        string template =
            $"""
            Прогресс

            Всего просмотренно комиксов: {progress.TotalComicsWatched}
            Текущая дата: {progress.LastWatchedComicDate:dd.MM.yyyy}
            Просмотренно {watched} комиксов из ~{totalComics}
            """;
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.MainMenu) },
        });

        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetSettingsMenu()
    {
        string template = "Настройки";
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Источники", CallbackDictionary.Sources) },
            new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.MainMenu) }
        });

        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetSourcesMenu(UserSettings settings)
    {
        string? acm = settings.Sources.HasFlag(SourceType.Acomics) ? " ✅" : null;
        string? acmB = settings.Sources.HasFlag(SourceType.AcomicsBegins) ? " ✅" : null;
        string? gcm = settings.Sources.HasFlag(SourceType.Gocomics) ? " ✅" : null;
        string? gcmB = settings.Sources.HasFlag(SourceType.GocomicsBegins) ? " ✅" : null;

        string template = "Источники";
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Acomics (RU)" + acm, CallbackDictionary.AcomicsSource) },
            new[] { InlineKeyboardButton.WithCallbackData("Acomics Begins (RU)" + acmB, CallbackDictionary.AcomicsBeginsSource) },
            new[] { InlineKeyboardButton.WithCallbackData("Gocomics (EN)" + gcm, CallbackDictionary.GocomicsSource) },
            new[] { InlineKeyboardButton.WithCallbackData("Gocomics Begins (EN)" + gcmB, CallbackDictionary.GocomicsBeginsSource) },
            new[] { InlineKeyboardButton.WithCallbackData("Назад", CallbackDictionary.Settings) }
        });

        return (template, keyboardMarkup);
    }
}
