namespace PeanutsEveryDay.Data.Models;

public class UserProgress
{
    public long UserId { get; init; }
    public int TotalComicsWatched { get; init; }

    public User? User { get; set; }
}
