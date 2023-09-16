namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

public abstract class CallbackDictionary
{
    public string SelectedItemLabel { get; } = " ✅";

    public abstract string MainMenu { get; }
    public abstract string Progress { get; }
    public abstract string Settings { get; }
    public abstract string Hide { get; }
    public abstract string Back { get; }

    public abstract string ProgressTemplate { get; }

    public abstract string Sources { get; }
    public abstract string Period { get; }
    public abstract string EveryHour { get; }
    public abstract string EveryDay { get; }
}
