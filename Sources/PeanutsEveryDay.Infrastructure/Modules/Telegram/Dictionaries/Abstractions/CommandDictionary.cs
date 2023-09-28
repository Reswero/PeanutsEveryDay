using Telegram.Bot.Types;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

public abstract class CommandDictionary
{
    public string Start = "/start";
    public string Help = "/help";
    public string SetKeyboard = "/keyboard";
    public string ComicByDate = "/date";
    public string SetDate = "/setdate";
    public string Next = "/next";
    public string MenuCommand = "/menu";
    public string StopSending = "/stop";

    public abstract string NextComic { get; }
    public abstract string Menu { get; }

    public abstract List<BotCommand> BotCommands { get; }
}
