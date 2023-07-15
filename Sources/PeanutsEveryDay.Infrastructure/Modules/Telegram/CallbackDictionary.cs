namespace PeanutsEveryDay.Infrastructure.Modules.Telegram;

public static class CallbackDictionary
{
    public const string Sources = "srcs";
    public const string SourcePrefix = "src_";
    public const string AcomicsSource = $"{SourcePrefix}acm";
    public const string AcomicsBeginsSource = $"{SourcePrefix}acmB";
    public const string GocomicsSource = $"{SourcePrefix}gcm";
    public const string GocomicsBeginsSource = $"{SourcePrefix}gcmB";
    public const string BackFromSources = "bckFrSrcs";

    public const string Hide = "hide";
}
