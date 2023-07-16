namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;

public static class CallbackDictionary
{
    public const string Sources = "srcs";
    public const string SourcePrefix = "src_";
    public const string AcomicsSource = $"{SourcePrefix}acm";
    public const string AcomicsBeginsSource = $"{SourcePrefix}acmB";
    public const string GocomicsSource = $"{SourcePrefix}gcm";
    public const string GocomicsBeginsSource = $"{SourcePrefix}gcmB";

    public const string MainMenu = "menu";
    public const string Progress = "progress";
    public const string Settings = "settings";
    public const string Hide = "hide";
}
