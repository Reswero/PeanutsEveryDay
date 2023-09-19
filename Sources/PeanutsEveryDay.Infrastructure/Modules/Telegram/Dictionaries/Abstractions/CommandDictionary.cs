namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

public abstract class CommandDictionary
{
    public string Start = "/start";
    public string Help = "/help";
    public string SetMenu = "/setmenu";
    public string ComicByDate = "/date";
    public string SetDate = "/setdate";

    public abstract string NextComic { get; }
    public abstract string Menu { get; }
}
