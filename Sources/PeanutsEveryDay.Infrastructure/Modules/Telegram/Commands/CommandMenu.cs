using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Telegram.Bot.Types;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class CommandMenu
{
    private static readonly List<BotCommand> _commands = new()
    {
        new BotCommand() { Command = "setmenu", Description = "Menu" },
        new BotCommand() { Command = "help", Description = "Help" }
    };

    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(Dom.User user, CancellationToken cancellationToken)
    {
        BotCommandScopeChat scope = new() { ChatId = user.Id };
        _senderService.EnqueueMessage(new CommandListMessage(user.Id, _commands, scope));
    }
}
