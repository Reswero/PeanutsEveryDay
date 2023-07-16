using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Dom = PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class ComicByDate
{
    private static IComicsService _comicsService;

    public static void Init(IComicsService comicsService)
    {
        _comicsService = comicsService;
    }

    public static async Task SendAsync(ITelegramBotClient bot, DateOnly date, Dom.User user, CancellationToken cancellationToken)
    {
        var comic = await _comicsService.GetComicAsync(date, SourceType.All, cancellationToken);

        if (comic is null)
        {
            await bot.SendTextMessageAsync(user.Id, AnswerDictionary.NoComicByDate, cancellationToken: cancellationToken);
            return;
        }

        string text = $"[{comic.PublicationDate:dd MMMM yyyy}]({comic.Url})";
        InputFileStream inputFile = new(comic.ImageStream, comic.PublicationDate.ToShortDateString());

        await bot.SendPhotoAsync(user.Id, inputFile, cancellationToken: cancellationToken);
        await bot.SendTextMessageAsync(user.Id, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true,
            cancellationToken: cancellationToken);

        user.Progress.IncreaseWatchedCount();
    }
}
