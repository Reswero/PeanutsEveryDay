using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Services;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
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

    public static async Task SendAsync(DateOnly date, User user, AnswerDictionary dictionary, CancellationToken cancellationToken)
    {
        var comic = await _comicsService.GetComicAsync(date, SourceType.All, cancellationToken);

        if (comic is null)
        {
            _senderService.EnqueueMessage(new TextMessage(user.Id, dictionary.NoComicByDate));
            return;
        }

        _senderService.EnqueueMessage(new ComicMessage(user.Id, user.Language, comic));

        user.Progress.IncreaseWatchedCount();
    }
}
