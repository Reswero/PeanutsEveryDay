namespace PeanutsEveryDay.Data.Models;

public class ParserState
{
    public int Id { get; set; }

    public int LastParsedAcomics { get; private set; } = 0;
    public int LastParsedAcomicsBegins { get; private set; } = 0;

    public DateOnly LastParsedGocomics { get; private set; } = new(1950, 10, 1);
    public DateOnly LastParsedGocomicsBegins { get; private set; } = new(1950, 10, 1);
}
