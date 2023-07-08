namespace PeanutsEveryDay.Data.Models;

public class User
{
    public long Id { get; init; }
    public required string FirstName { get; init; }
    public string? Username { get; init; }

    public UserProgress? Progress { get; init; }
    public UserSettings? Settings { get; init; }
}
