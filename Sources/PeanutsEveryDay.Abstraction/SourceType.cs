namespace PeanutsEveryDay.Abstraction;

/// <summary>
/// Type of comics source
/// </summary>
[Flags]
public enum SourceType
{
    Acomics = 1,
    AcomicsBegins = 2,
    Gocomics = 4,
    GocomicsBegins = 8
}
