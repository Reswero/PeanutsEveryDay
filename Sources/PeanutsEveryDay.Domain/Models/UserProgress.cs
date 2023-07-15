namespace PeanutsEveryDay.Domain.Models;

public class UserProgress
{
    private DateOnly _lastWatchedComicDate = new(1950, 10, 01);

    public required long UserId { get; init; }
    public DateOnly LastWatchedComicDate
    {
        get => _lastWatchedComicDate;
        init => _lastWatchedComicDate = value;
    }
    public int TotalComicsWatched { get; init; }

    public void IncreaseDate()
    {
        _lastWatchedComicDate = LastWatchedComicDate.AddDays(1);
    }
}
