namespace PeanutsEveryDay.Domain.Models;

public class UserProgress
{
    private int _totalComicsWatched = default;
    private DateOnly _lastWatchedComicDate = new(1950, 10, 01);

    public required long UserId { get; init; }
    public int TotalComicsWatched
    {
        get => _totalComicsWatched;
        init => _totalComicsWatched = value;
    }
    public DateOnly LastWatchedComicDate
    {
        get => _lastWatchedComicDate;
        init => _lastWatchedComicDate = value;
    }

    public void SetDate(DateOnly date)
    {
        _lastWatchedComicDate = date;
    }

    public void IncreaseWatchedCount()
    {
        _totalComicsWatched += 1;
    }
}
