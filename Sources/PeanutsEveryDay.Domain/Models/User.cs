namespace PeanutsEveryDay.Domain.Models;

public class User
{
    public required long Id { get; init; }
    public required string FirstName { get; init; }
    public string? Username { get; init; }

    public required UserProgress Progress { get; init; }
    public required UserSettings Settings { get; init; }

    public static User Create(long Id, string firstName, string? username)
    {
        UserProgress progress = new() { UserId = Id };
        UserSettings settings = new() { UserId = Id };
        User user = new()
        {
            Id = Id,
            FirstName = firstName,
            Username = username,
            Progress = progress,
            Settings = settings
        };

        return user;
    }
}
