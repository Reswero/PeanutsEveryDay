using PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class SetDate
{
    private static readonly DateOnly _minDate = new(1950, 10, 01);

    public static async Task ExecuteAsync(DateOnly date, User user, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        if (date > currentDate)
        {
            date = currentDate;
        }
        else if (date < _minDate)
        {
            date = _minDate;
        }

        user.Progress.SetDate(date);
    }
}
