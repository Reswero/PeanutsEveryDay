namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;

public static class CallbackKey
{
    public const string Sources = "srcs";
    public const string SourcePrefix = "src_";
    public const string AcomicsSource = $"{SourcePrefix}acm";
    public const string AcomicsBeginsSource = $"{SourcePrefix}acmB";
    public const string GocomicsSource = $"{SourcePrefix}gcm";
    public const string GocomicsBeginsSource = $"{SourcePrefix}gcmB";
    public const string SourcesInfo = "srcs_inf";

    public const string Period = "period";
    public const string PeriodPrefix = "prd_";
    public const string EveryHourPeriod = $"{PeriodPrefix}eh";
    public const string EveryDayPeriod = $"{PeriodPrefix}ed";

    public const string MainMenu = "menu";
    public const string Progress = "progress";
    public const string Settings = "settings";
    public const string Hide = "hide";
}
