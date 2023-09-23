using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

public static class CallbackHelper
{
    public static void ChangeSources(string callback, UserSettings settings)
    {
        switch (callback)
        {
            case CallbackKey.AcomicsSource:
                settings.InverseSource(SourceType.Acomics);
                break;
            case CallbackKey.AcomicsBeginsSource:
                settings.InverseSource(SourceType.AcomicsBegins);
                break;
            case CallbackKey.GocomicsSource:
                settings.InverseSource(SourceType.Gocomics);
                break;
            case CallbackKey.GocomicsBeginsSource:
                settings.InverseSource(SourceType.GocomicsBegins);
                break;
        }
    }

    public static void ChangePeriod(string callback, UserSettings settings)
    {
        switch (callback)
        {
            case CallbackKey.EveryHourPeriod:
                settings.SetPeriod(PeriodType.EveryHour);
                break;
            case CallbackKey.EveryDayPeriod:
                settings.SetPeriod(PeriodType.EveryDay);
                break;
        }
    }

    public static (string?, InlineKeyboardMarkup?) GetTemplateWithKeyboardMarkup(string callback, User user,
        CallbackDictionary dictionary)
    {
        return callback switch
        {
            CallbackKey.MainMenu => GetMainMenu(dictionary),
            CallbackKey.Progress => GetProgressMenu(user, dictionary),
            CallbackKey.Settings => GetSettingsMenu(dictionary),
            CallbackKey.Sources => GetSourcesMenu(user, dictionary),
            CallbackKey.SourcesInfo => GetSourcesInfoMenu(dictionary),
            CallbackKey.Period => GetPeriodMenu(user.Settings, dictionary),
            _ => (null, null),
        };
    }

    private static (string, InlineKeyboardMarkup) GetMainMenu(CallbackDictionary dictionary)
    {
        string template = dictionary.MainMenu;
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Progress, CallbackKey.Progress) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Settings, CallbackKey.Settings) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Hide, CallbackKey.Hide) }
        });
        
        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetProgressMenu(User user, CallbackDictionary dictionary)
    {
        var progress = user.Progress;

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var startDate = new DateOnly(1950, 10, 01);

        var watched = progress.LastWatchedComicDate.DayNumber - startDate.DayNumber;
        var totalComics = currentDate.DayNumber - startDate.DayNumber;

        string lastWatchedComicDate = DateUtils.ConvertDate(progress.LastWatchedComicDate, user.Language);
        string template = string.Format(dictionary.ProgressTemplate, progress.TotalComicsWatched,
            lastWatchedComicDate, watched, totalComics);
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Back, CallbackKey.MainMenu) },
        });

        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetSettingsMenu(CallbackDictionary dictionary)
    {
        string template = dictionary.Settings;
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Sources, CallbackKey.Sources) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.SendingPeriod, CallbackKey.Period) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Back, CallbackKey.MainMenu) }
        });

        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetSourcesMenu(User user, CallbackDictionary dictionary)
    {
        var settings = user.Settings;

        string? gcm = settings.Sources.HasFlag(SourceType.Gocomics) ? dictionary.SelectedItemLabel : null;
        string? gcmB = settings.Sources.HasFlag(SourceType.GocomicsBegins) ? dictionary.SelectedItemLabel : null;

        string template = dictionary.Sources;

        List<InlineKeyboardButton[]> buttons = new(5);
        if (user.Language == LanguageCode.Ru)
        {
            string? acm = settings.Sources.HasFlag(SourceType.Acomics) ? dictionary.SelectedItemLabel : null;
            string? acmB = settings.Sources.HasFlag(SourceType.AcomicsBegins) ? dictionary.SelectedItemLabel : null;

            buttons.AddRange(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Acomics (RU)" + acm, CallbackKey.AcomicsSource) },
                new[] { InlineKeyboardButton.WithCallbackData("Acomics Begins (RU)" + acmB, CallbackKey.AcomicsBeginsSource) }
            });
        }

        buttons.AddRange(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Gocomics (EN)" + gcm, CallbackKey.GocomicsSource) },
            new[] { InlineKeyboardButton.WithCallbackData("Gocomics Begins (EN)" + gcmB, CallbackKey.GocomicsBeginsSource) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.SourcesInfo, CallbackKey.SourcesInfo) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Back, CallbackKey.Settings) }
        });

        InlineKeyboardMarkup keyboardMarkup = new(buttons);
        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetSourcesInfoMenu(CallbackDictionary dictionary)
    {
        string template = dictionary.SourcesInfoTemplate;
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Back, CallbackKey.Sources) }
        });

        return (template, keyboardMarkup);
    }

    private static (string, InlineKeyboardMarkup) GetPeriodMenu(UserSettings settings, CallbackDictionary dictionary)
    {
        string? eh = settings.Period.HasFlag(PeriodType.EveryHour) ? dictionary.SelectedItemLabel : null;
        string? ed = settings.Period.HasFlag(PeriodType.EveryDay) ? dictionary.SelectedItemLabel : null;

        string template = dictionary.SendingPeriod;
        InlineKeyboardMarkup keyboardMarkup = new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.EveryHour + eh, CallbackKey.EveryHourPeriod) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.EveryDay + ed, CallbackKey.EveryDayPeriod) },
            new[] { InlineKeyboardButton.WithCallbackData(dictionary.Back, CallbackKey.Settings) }
        });

        return (template, keyboardMarkup);
    }
}
