using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class ComicByDate
{
    private static IComicsService _comicsService;
    private static MessagesSenderService _senderService;

    public static void Init(IComicsService comicsService, MessagesSenderService senderService)
    {
        _comicsService = comicsService;
        _senderService = senderService;
    }

    public static async Task SendAsync(DateOnly date, User user, CancellationToken cancellationToken)
    {
        var comic = await _comicsService.GetComicAsync(date, SourceType.All, cancellationToken);

        if (comic is null)
        {
            _senderService.EnqueueMessage(new TextMessage(user.Id, AnswerDictionary.NoComicByDate));
            return;
        }

        _senderService.EnqueueMessage(new ComicMessage(user.Id, comic));

        user.Progress.IncreaseWatchedCount();
    }
}
