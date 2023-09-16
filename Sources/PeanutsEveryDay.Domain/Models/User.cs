using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class User
{
    public required long Id { get; init; }
    public required string FirstName { get; init; }
    public string? Username { get; init; }

    public LanguageCode Language { get; init; }

    public required UserProgress Progress { get; init; }
    public required UserSettings Settings { get; init; }

    public static User Create(long id, string firstName, string? username, string? language)
    {
        LanguageCode languageCode;
        
        if (language == "ru" || language == "be" ||
            language == "kk" || language == "uk")
        {
            languageCode = LanguageCode.Ru;
        }
        else
        {
            languageCode = LanguageCode.En;
        }

        UserProgress progress = new() { UserId = id };
        UserSettings settings = UserSettings.Create(id, languageCode);
        User user = new()
        {
            Id = id,
            FirstName = firstName,
            Username = username,
            Language = languageCode,
            Progress = progress,
            Settings = settings
        };

        return user;
    }
}
