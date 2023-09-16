using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class UserSettings
{
    private SourceType _sources;
    private PeriodType _period = PeriodType.EveryHour;

    public long UserId { get; init; }
    public SourceType Sources
    {
        get => _sources;
        init => _sources = value;
    }
    public PeriodType Period
    {
        get => _period;
        init => _period = value;
    }

    public static UserSettings Create(long userId, LanguageCode language)
    {
        SourceType sources;

        if (language == LanguageCode.Ru)
        {
            sources = SourceType.Acomics | SourceType.AcomicsBegins |
                SourceType.Gocomics | SourceType.GocomicsBegins;
        }
        else
        {
            sources = SourceType.Gocomics | SourceType.GocomicsBegins;
        }

        return new UserSettings
        {
            UserId = userId,
            Sources = sources
        };
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

    public void SetPeriod(PeriodType period)
    {
        if ((Period == PeriodType.EveryHour && period == PeriodType.EveryHour) ||
            (Period == PeriodType.EveryDay && period == PeriodType.EveryDay))
        {
            _period = PeriodType.None;
        }
        else if (Period == PeriodType.EveryHour && period == PeriodType.EveryDay)
        {
            _period = PeriodType.EveryDay;
        }
        else
        {
            _period = PeriodType.EveryHour;
        }
    }
}
