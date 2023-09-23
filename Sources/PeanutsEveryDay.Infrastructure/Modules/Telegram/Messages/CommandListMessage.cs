using Telegram.Bot.Types;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class CommandListMessage : AbstractMessage
{
    public IReadOnlyCollection<BotCommand> Commands { get; private set; }
    public BotCommandScope CommandsScope { get; private set; }

    public CommandListMessage(long userId, IReadOnlyCollection<BotCommand> commands,
        BotCommandScope commandScope) : base(userId)
    {
        Commands = commands;
        CommandsScope = commandScope;
    }
}
