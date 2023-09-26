using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Persistence;
using Timer = System.Timers.Timer;

namespace PeanutsEveryDay.Infrastructure.Modules.Metrics;

public static class SimpleMetricsService
{
    private readonly static Timer _interimSavingTimer;
    private readonly static Timer _dailyRestartingTimer;

    private static int _registeredUsers = default;
    private static long _sendedComics = default;

    private static IServiceProvider _serviceProvider;

    static SimpleMetricsService()
    {
        _interimSavingTimer = new()
        {
            AutoReset = true,
            Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
        };
        _interimSavingTimer.Elapsed += InterimSavingTimer_Elapsed;
        _interimSavingTimer.Start();

        _dailyRestartingTimer = new()
        {
            AutoReset = true,
            Interval = GetNextDay().TotalMilliseconds
        };
        _dailyRestartingTimer.Elapsed += DailyRestartingTimer_Elapsed;
        _dailyRestartingTimer.Start();
    }

    public static void Init(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeanutsContext>();

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);

        var currentMetric = db.Metrics.FirstOrDefault(m => m.Date == date);

        if (currentMetric is not null)
        {
            _registeredUsers = currentMetric.RegisteredUsers;
            _sendedComics = currentMetric.SendedComics;
        }
    }

    public static void IncreaseUsers()
    {
        Interlocked.Increment(ref _registeredUsers);
    }

    public static void IncreaseWatchedComics()
    {
        Interlocked.Increment(ref _sendedComics);
    }

    public static async Task<List<Metric>> GetMetricsAsync(DateOnly from, DateOnly to)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeanutsContext>();

        List<Metric> metrics = await db.Metrics.Where(m => m.Date >= from && m.Date < to).ToListAsync();

        return metrics;
    }

    public static async Task SaveMetricAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PeanutsContext>();

        DateOnly date = DateOnly.FromDateTime(DateTime.Now);
        int reg = _registeredUsers;
        long sen = _sendedComics;
        Metric metric = new(date, reg, sen);

        await db.BulkInsertOrUpdateAsync(new[] { metric });
        await db.BulkSaveChangesAsync();
    }

    private static async void InterimSavingTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await SaveMetricAsync();
    }

    private static async void DailyRestartingTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await RestartDayAsync();
    }

    private static async Task RestartDayAsync()
    {
        await SaveMetricAsync();

        Interlocked.Exchange(ref _registeredUsers, 0);
        Interlocked.Exchange(ref _sendedComics, 0);

        _dailyRestartingTimer.Interval = GetNextDay().TotalMilliseconds;
    }

    private static TimeSpan GetNextDay()
    {
        DateTime now = DateTime.Now;
        DateTime nextDay = new DateTime(now.Year, now.Month, now.Day, 01, 30, 00).AddDays(1);

        return nextDay - now;
    }
}
