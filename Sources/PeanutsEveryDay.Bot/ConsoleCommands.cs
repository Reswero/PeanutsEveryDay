using PeanutsEveryDay.Infrastructure.Modules.Metrics;

namespace PeanutsEveryDay.Bot;

public static class ConsoleCommands
{
    public static async Task ExecuteQuitCommand()
    {
        await SimpleMetricsService.SaveMetricAsync();
        Environment.Exit(0);
    }

    public static async Task ExecuteMetricsCommand(string command)
    {
        var args = command.Split(' ');

        if (args.Length < 3)
        {
            Console.WriteLine("Not enough args.");
            return;
        }

        var fromParsed = DateOnly.TryParse(args[1], out var from);
        var toParsed = DateOnly.TryParse(args[2], out var to);

        if (fromParsed is false || toParsed is false)
        {
            Console.WriteLine("Wrong date format.");
            return;
        }

        if (args.Length < 4 || string.Equals(args[3], "--console", StringComparison.InvariantCultureIgnoreCase))
        {
            await SimpleMetricsViewer.PrintToConsoleAsync(from, to);
        }
        else if (args.Length < 4 || string.Equals(args[3], "--file", StringComparison.InvariantCultureIgnoreCase))
        {
            await SimpleMetricsViewer.SaveToFileAsync(from, to);
        }
        else
        {
            await Console.Out.WriteLineAsync("Unsupported format.");
        }
    }
}
