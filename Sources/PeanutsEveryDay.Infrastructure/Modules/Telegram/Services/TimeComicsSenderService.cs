﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using Telegram.Bot;
using Timer = System.Timers.Timer;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

public class TimeComicsSenderService
{
    private readonly ITelegramBotClient _bot;
    private readonly IServiceProvider _provider;
    private readonly ILogger<TimeComicsSenderService> _logger;

    private Timer _hourTimer;
    private Timer _dayTimer;

    public TimeComicsSenderService(ITelegramBotClient bot, IServiceProvider provider)
    {
        _bot = bot;
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
        DateTime now = DateTime.Now.Date;
        var diff = now.AddDays(1).AddHours(10).AddMinutes(30) - now;
        return diff;
    }

    private async void HourTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Hourly sending started.");

        await SendComics(PeriodType.EveryHour);
        _hourTimer.Interval = NextHour().TotalMilliseconds;

        _logger.LogInformation("Hourly sending ended.");
    }

    private async void DayTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _logger.LogInformation("Daily sending started.");

        await SendComics(PeriodType.EveryDay);
        _dayTimer.Interval = NextDay().TotalMilliseconds;

        _logger.LogInformation("Daily sending ended.");
    }

    private async Task SendComics(PeriodType period)
    {
        var repository = _provider.CreateScope().ServiceProvider.GetRequiredService<IUsersRepository>();
        var users = await repository.GetByFilterAsync(u => u.Settings.Period == period);

        foreach (var user in users)
        {
            await NextComic.SendAsync(_bot, user, CancellationToken.None);
            await repository.UpdateAsync(user, CancellationToken.None);
        }
    }
}
