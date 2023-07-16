using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class NextComic
{
    private static IComicsService _comicsService;

    public static void Init(IComicsService comicsService)
    {
        _comicsService = comicsService;
    }

    public static async Task SendAsync(ITelegramBotClient bot, Dom.User user, CancellationToken cancellationToken)
    {
        if (user.Settings.Sources == SourceType.None)
        {
            await bot.SendTextMessageAsync(user.Id, "Должен быть выбран хотя бы один источник с комиксами!",
                cancellationToken: cancellationToken);
            return;
        }

        var nextDate = user.Progress.LastWatchedComicDate.AddDays(1);
        var comic = await _comicsService.GetComicAsync(nextDate, user.Settings.Sources, cancellationToken);

        if (comic is null)
        {
            nextDate = nextDate.AddDays(1);
            comic = await _comicsService.GetComicAsync(nextDate, user.Settings.Sources, cancellationToken);

            if (comic is null)
            {
                await bot.SendTextMessageAsync(user.Id, "Комиксы закончились :(", cancellationToken: cancellationToken);
                return;
            }

        }

        string text = $"[{comic.PublicationDate:dd MMMM yyyy}]({comic.Url})";
        InputFileStream inputFile = new(comic.ImageStream, comic.PublicationDate.ToShortDateString());

        await bot.SendPhotoAsync(user.Id, inputFile, cancellationToken: cancellationToken);
        await bot.SendTextMessageAsync(user.Id, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        user.Progress.SetDate(nextDate);
        user.Progress.IncreaseWatchedCount();
    }
}
