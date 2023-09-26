using ConsoleTableExt;
using System.Text;

namespace PeanutsEveryDay.Infrastructure.Modules.Metrics;

public static class SimpleMetricsViewer
{
    public static async Task PrintToConsoleAsync(DateOnly from, DateOnly to)
    {
        string table = await MakeTableAsync(from, to);
        await Console.Out.WriteLineAsync(table);
    }

    public static async Task SaveToFileAsync(DateOnly from, DateOnly to)
    {
        Directory.CreateDirectory("metrics");
        string filePath = Path.Combine("metrics", $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt");

        string table = await MakeTableAsync(from, to);
        await File.WriteAllTextAsync(filePath, table);
    }

    private static async Task<string> MakeTableAsync(DateOnly from, DateOnly to)
    {
        var metrics = await SimpleMetricsService.GetMetricsAsync(from, to);

        List<List<object>> tableData = new(metrics.Count);

        foreach (var metr in metrics)
        {
            List<object> row = new() { metr.Date, metr.RegisteredUsers, metr.SendedComics };
            tableData.Add(row);
        }

        List<object> totallyRow = new() { "Totally", metrics.Sum(m => m.RegisteredUsers), metrics.Sum(m => m.SendedComics) };
        tableData.Add(totallyRow);

        StringBuilder table = ConsoleTableBuilder
            .From(tableData)
            .AddColumn("Date", "Registered Users", "Watched Comics")
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .WithTextAlignment(new()
            {
                    { 1, TextAligntment.Right },
                    { 2, TextAligntment.Right }
            })
            .Export();

        return table.ToString();
    }
}
