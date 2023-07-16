using PeanutsEveryDay.Domain.Models;
using Telegram.Bot;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class SetDate
{
    public static async Task ExecuteAsync(ITelegramBotClient bot, DateOnly date, User user,
        CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if (date > currentDate)
        {
            date = currentDate;
        }

        user.Progress.SetDate(date);
    }
}
