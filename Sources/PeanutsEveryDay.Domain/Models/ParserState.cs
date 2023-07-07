namespace PeanutsEveryDay.Domain.Models;

/// <summary>
/// Comics parsers state
/// </summary>
// This is a bad solution because it is tied to a specific parser implementation.
// But how to properly distinguish between different types of parser states I haven't figured out
public class ParserState
{
    /// <summary>
    /// Last parsed comic number from Acomics
    /// </summary>
    public int LastParsedAcomics { get; private set; } = 0;
    /// <summary>
    /// Last parsed comic number from Acomics Begins
    /// </summary>
    public int LastParsedAcomicsBegins { get; private set; } = 0;

    /// <summary>
    /// Last parsed comic date from Gocomics
    /// </summary>
    public DateOnly LastParsedGocomics { get; private set; } = new(1950, 10, 1);
    /// <summary>
    /// Last parsed comic date from Gocomics Begins
    /// </summary>
    public DateOnly LastParsedGocomicsBegins { get; private set; } = new(1950, 10, 1);

    /// <summary>
    /// Changing Acomics parser state
    /// </summary>
    /// <param name="lastParsedNumber">Last parsed comic number</param>
    /// <param name="fromBegins">Comic from Begins collection</param>
    public void ChangeAcomics(int lastParsedNumber, bool fromBegins)
    {
        if (fromBegins is true)
        {
            LastParsedAcomicsBegins = lastParsedNumber;
        }
        else
        {
            LastParsedAcomics = lastParsedNumber;
        }
    }

    /// <summary>
    /// Changing Gocomics parser state
    /// </summary>
    /// <param name="lastParsedDate">Last parsed comic date</param>
    /// <param name="fromBegins">Comic from Begins collection</param>
    public void ChangeGocomics(DateOnly lastParsedDate, bool fromBegins)
    {
        if (fromBegins is true)
        {
            LastParsedGocomicsBegins = lastParsedDate;
        }
        else
        {
            LastParsedGocomics = lastParsedDate;
        }
    }
}
