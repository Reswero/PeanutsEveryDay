namespace PeanutsEveryDay.Data.Models;

public class ParserState
{
    public int Id { get; set; }

    public int LastParsedAcomics { get; set; } = 0;
    public int LastParsedAcomicsBegins { get; set; } = 0;

    public DateOnly LastParsedGocomics { get; set; } = new(1950, 10, 1);
    public DateOnly LastParsedGocomicsBegins { get; set; } = new(1950, 10, 1);
}
