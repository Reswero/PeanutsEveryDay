using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class SetDate
{
    private static readonly DateOnly _firstComicDate = new(1950, 10, 02);

    public static async Task ExecuteAsync(DateOnly date, User user, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if (date > currentDate)
        {
            date = currentDate;
        }
        else if (date < _firstComicDate)
        {
            date = _firstComicDate;
        }

        user.Progress.SetDate(date);
    }
}
