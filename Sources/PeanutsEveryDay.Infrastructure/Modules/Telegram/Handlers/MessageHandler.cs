using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Telegram.Bot.Types;
using Dom = PeanutsEveryDay.Domain.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;

public static class MessageHandler
{
    private static IServiceProvider _serviceProvider;
    private static MessagesSenderService _messagesSenderService;

    public static void Init(IServiceProvider serviceProvider, MessagesSenderService messagesSenderService)
    {
        _serviceProvider = serviceProvider;
        _messagesSenderService = messagesSenderService;
    }

    public static async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = message.From!.Id;
        var user = await repository.GetAsync(chatId, cancellationToken);

        if (user is null)
        {
            user = Dom.User.Create(chatId, message.From.FirstName, message.From.Username);
            await repository.AddAsync(user, cancellationToken);
        }

        if (message.Text == CommandDictionary.Start ||
            message.Text == CommandDictionary.SetMenu)
        {
            await KeyboardMenu.SendAsync(user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.NextComic)
        {
            await NextComic.SendAsync(user, cancellationToken);
            await repository.UpdateAsync(user, cancellationToken);
        }
        else if (message.Text == CommandDictionary.Menu)
        {
            await MainMenu.SendAsync(user, cancellationToken);
        }
        else if (message.Text.StartsWith(CommandDictionary.ComicByDate))
        {
            string textDate = message.Text[CommandDictionary.ComicByDate.Length..];
            var parsed = DateOnly.TryParse(textDate, out var date);

            if (parsed is true)
            {
                await ComicByDate.SendAsync(date, user, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                _messagesSenderService.EnqueueMessage(new TextMessage(user.Id, AnswerDictionary.WrongDateFormat));
            }
        }
        else if (message.Text.StartsWith(CommandDictionary.SetDate))
        {
            string textDate = message.Text[CommandDictionary.SetDate.Length..];
            var parsed = DateOnly.TryParse(textDate, out var date);

            if (parsed is true)
            {
                await SetDate.ExecuteAsync(date, user, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                _messagesSenderService.EnqueueMessage(new TextMessage(user.Id, AnswerDictionary.WrongDateFormat));
            }
        }
    }
}
