namespace PeanutsEveryDay.Data.Models;

public class User
{
    public required long Id { get; init; }
    public required string FirstName { get; init; }
    public string? Username { get; init; }
}
