using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using Telegram.Bot.Types;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Ru;

public class RuCommandDictionary : CommandDictionary
{
    public override string NextComic => "Следующий ➡️";
    public override string Menu => "Меню 📋";

    public override List<BotCommand> BotCommands => new()
    {
        new BotCommand() { Command = "help", Description = "Информация о доступных командах" },
        new BotCommand() { Command = "keyboard", Description = "Установка наэкранной клавиатуры" },
        new BotCommand() { Command = "menu", Description = "Меню бота" },
        new BotCommand() { Command = "next", Description = "Получение следующего комикса" },
        new BotCommand() { Command = "stop", Description = "Остановить рассылку комиксов" }
    };
}
