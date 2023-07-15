using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class UserSettings
{
    private SourceType _sources = SourceType.AcomicsBegins | SourceType.Acomics |
        SourceType.GocomicsBegins | SourceType.Gocomics;

    public long UserId { get; init; }
    public SourceType Sources
    {
        get => _sources;
        init => _sources = value;
    }

    public void InverseSource(SourceType source)
    {
        if (_sources.HasFlag(source))
        {
            _sources &= ~source;
        }
        else
        {
            _sources |= source;
        }
    }
}
