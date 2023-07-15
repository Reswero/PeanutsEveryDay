using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class UserSettings
{
    public long UserId { get; init; }
    public SourceType Sources { get; init; } = SourceType.AcomicsBegins | SourceType.Acomics |
        SourceType.GocomicsBegins | SourceType.Gocomics;
}
