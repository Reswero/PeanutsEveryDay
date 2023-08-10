using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class SetDate
{
    public static async Task ExecuteAsync(DateOnly date, User user, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if (date > currentDate)
        {
            date = currentDate;
        }

        user.Progress.SetDate(date);
    }
}
