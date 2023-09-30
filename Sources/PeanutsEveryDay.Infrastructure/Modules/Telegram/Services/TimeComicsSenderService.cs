using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using Timer = System.Timers.Timer;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

public class TimeComicsSenderService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TimeComicsSenderService> _logger;

    private Timer _hourTimer;
    private Timer _dayTimer;

    public TimeComicsSenderService(IServiceProvider provider)
    {
        _provider = provider;
        _logger = provider.GetRequiredService<ILogger<TimeComicsSenderService>>();
    }

    public void Start()
    {
        _hourTimer = new Timer()
        {
            AutoReset = true,
            Enabled = true,
            Interval = NextHour().TotalMilliseconds
        };
        _hourTimer.Elapsed += HourTimer_Elapsed;
        _hourTimer.Start();

        _dayTimer = new Timer()
        {
            AutoReset = true,
            Enabled = true,
            Interval = NextDay().TotalMilliseconds
        };
        _dayTimer.Elapsed += DayTimer_Elapsed;
        _dayTimer.Start();
    }

    private static TimeSpan NextHour()
    {
        DateTime now = DateTime.Now;
        var diff = 60 - now.Minute;
        return TimeSpan.FromMinutes(diff);
    }

    private static TimeSpan NextDay()
    {
        DateTime now = DateTime.Now;
        var diff = now.Date.AddDays(1).AddHours(14).AddMinutes(30) - now;
        return diff;
    }

    private async void HourTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Hourly sending started.");

        _hourTimer.Interval = NextHour().TotalMilliseconds;
        await SendComics(PeriodType.EveryHour);

        _logger.LogInformation("Hourly sending ended.");
    }

    private async void DayTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Daily sending started.");

        _dayTimer.Interval = NextDay().TotalMilliseconds;
        await SendComics(PeriodType.EveryDay);

        _logger.LogInformation("Daily sending ended.");
    }

    private async Task SendComics(PeriodType period)
    {
        var scope = _provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var users = await repository.GetByFilterAsync(u => u.Settings.Period == period &&
                                                           u.Settings.Sources != SourceType.None);

        foreach (var user in users)
        {
            var dictionary = scope.ServiceProvider.GetRequiredService<AnswerDictionaryResolver>().Invoke(user.Language);

            await NextComic.SendAsync(user, dictionary, sendComicsOut: false, CancellationToken.None);
            await repository.UpdateAsync(user, CancellationToken.None);
        }
    }
}
