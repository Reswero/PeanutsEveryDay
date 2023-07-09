namespace PeanutsEveryDay.Domain.Models;

/// <summary>
/// Comics parsers state
/// </summary>
// This is a bad solution because it is tied to a specific parser implementation.
// But how to properly distinguish between different types of parser states I haven't figured out
public class ParserState
{
    private int _lastParsedAcomics = 0;
    private int _lastParsedAcomicsBegins = 0;
    private DateOnly _lastParsedGocomics = new(1950, 10, 01);
    private DateOnly _lastParsedGocomicsBegins = new(1950, 10, 01);

    /// <summary>
    /// Last parsed comic number from Acomics
    /// </summary>
    public int LastParsedAcomics
    {
        get => _lastParsedAcomics;
        init => _lastParsedAcomics = value;
    }
    /// <summary>
    /// Last parsed comic number from Acomics Begins
    /// </summary>
    public int LastParsedAcomicsBegins
    {
        get => _lastParsedAcomicsBegins;
        init => _lastParsedAcomicsBegins = value;
    }

    /// <summary>
    /// Last parsed comic date from Gocomics
    /// </summary>
    public DateOnly LastParsedGocomics
    {
        get => _lastParsedGocomics;
        init => _lastParsedGocomics = value;
    }
    /// <summary>
    /// Last parsed comic date from Gocomics Begins
    /// </summary>
    public DateOnly LastParsedGocomicsBegins
    {
        get => _lastParsedGocomicsBegins;
        init => _lastParsedGocomicsBegins = value;
    }

    /// <summary>
    /// Changing Acomics parser state
    /// </summary>
    /// <param name="lastParsedNumber">Last parsed comic number</param>
    /// <param name="fromBegins">Comic from Begins collection</param>
    public void ChangeAcomics(int lastParsedNumber, bool fromBegins)
    {
        if (fromBegins is true)
        {
            _lastParsedAcomicsBegins = lastParsedNumber;
        }
        else
        {
            _lastParsedAcomics = lastParsedNumber;
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
            _lastParsedGocomicsBegins = lastParsedDate;
        }
        else
        {
            _lastParsedGocomics = lastParsedDate;
        }
    }
}
