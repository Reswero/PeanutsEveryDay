using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public class StopSending
{
    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task ExecuteAsync(Dom.User user, AnswerDictionary answerDictionary, CancellationToken cancellationToken)
    {
        user.Settings.SetPeriod(Abstraction.PeriodType.None);

        _senderService.EnqueueMessage(new TextMessage(user.Id, answerDictionary.SendingStopped));
    }
}
