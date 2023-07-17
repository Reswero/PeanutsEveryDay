using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class UserSettings
{
    private SourceType _sources = SourceType.AcomicsBegins | SourceType.Acomics |
        SourceType.GocomicsBegins | SourceType.Gocomics;
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
