using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using Telegram.Bot.Types;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.En;

public class EnCommandDictionary : CommandDictionary
{
    public override string NextComic => "Next ➡️";
    public override string Menu => "Menu 📋";

    public override List<BotCommand> BotCommands => new()
    {
        new BotCommand() { Command = "help", Description = "Information about available commands" },
        new BotCommand() { Command = "keyboard", Description = "Installing the on-screen keyboard" },
        new BotCommand() { Command = "menu", Description = "Bot menu" },
        new BotCommand() { Command = "next", Description = "Getting the next comic" },
        new BotCommand() { Command = "stop", Description = "Stop sending comics" }
    };
}
