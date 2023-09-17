using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.En;

public class EnCallbackDictionary : CallbackDictionary
{
    public override string MainMenu => "Menu";
    public override string Progress => "Progress";
    public override string Settings => "Settings";
    public override string Hide => "Hide";
    public override string Back => "Back";

    public override string ProgressTemplate =>
            """
            Progress

            Total comics viewed: {0}
            Current date: {1}
            Viewed {2} comics out of ~{3}
            """;

    public override string Sources => "Sources";
    public override string Period => "Period";
    public override string EveryHour => "Every hour";
    public override string EveryDay => "Every day";
}
