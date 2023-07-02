namespace PeanutsEveryDay.Abstraction;

/// <summary>
/// Comics sending period
/// </summary>
[Flags]
public enum PeriodType
{
    None = 0,
    EveryHour = 1,
    EveryDay = 2
}
