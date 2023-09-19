using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class HelpInfo
{
    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(Dom.User user, AnswerDictionary dictionary, CancellationToken cancellationToken)
    {
        _senderService.EnqueueMessage(new TextMessage(user.Id, dictionary.HelpInformation));
    }
}
