using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class NextComic
{
    private static IComicsService _comicsService;
    private static MessagesSenderService _senderService;

    public static void Init(IComicsService comicsService, MessagesSenderService senderService)
    {
        _comicsService = comicsService;
        _senderService = senderService;
    }

    public static async Task SendAsync(User user, AnswerDictionary dictionary, CancellationToken cancellationToken)
    {
        if (user.Settings.Sources == SourceType.None)
        {
            _senderService.EnqueueMessage(new TextMessage(user.Id, dictionary.NeededAtLeastOneSource));
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
                _senderService.EnqueueMessage(new TextMessage(user.Id, dictionary.ComicsOut));
                return;
            }
        }

        _senderService.EnqueueMessage(new ComicMessage(user.Id, user.Language, comic));

        user.Progress.SetDate(nextDate);
        user.Progress.IncreaseWatchedCount();
    }
}
