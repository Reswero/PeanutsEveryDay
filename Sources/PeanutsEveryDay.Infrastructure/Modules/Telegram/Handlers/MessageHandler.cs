using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using PeanutsEveryDay.Infrastructure.Modules.Utils;
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

        long userId = message.From!.Id;

        if (AntiSpamService.IsUserRequestInProcess(userId))
        {
            return;
        }
        AntiSpamService.ProcessUserRequest(userId);

        try
        {
            await ProcessAsync(userId, message, cancellationToken);
        }
        catch
        {
            AntiSpamService.UserRequestProcessed(userId);
            throw;
        }
    }

    private static async Task ProcessAsync(long userId, Message message, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        var user = await repository.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            user = Dom.User.Create(userId, message.From.FirstName, message.From.Username, message.From.LanguageCode);
            await repository.AddAsync(user, cancellationToken);
        }

        var commandDictionary = scope.ServiceProvider.GetRequiredService<CommandDictionaryResolver>().Invoke(user.Language);
        var answerDictionary = scope.ServiceProvider.GetRequiredService<AnswerDictionaryResolver>().Invoke(user.Language);

        if (message.Text == commandDictionary.Start ||
            message.Text == commandDictionary.SetMenu)
        {
            await KeyboardMenu.SendAsync(user, commandDictionary, answerDictionary, cancellationToken);
            await CommandMenu.SendAsync(user, cancellationToken);
        }
        else if (message.Text == commandDictionary.NextComic)
        {
            await NextComic.SendAsync(user, answerDictionary, cancellationToken);
            await repository.UpdateAsync(user, cancellationToken);
        }
        else if (message.Text == commandDictionary.Menu)
        {
            var callbackDictionary = scope.ServiceProvider.GetRequiredService<CallbackDictionaryResolver>().Invoke(user.Language);
            await MainMenu.SendAsync(user, callbackDictionary, cancellationToken);
        }
        else if (message.Text == commandDictionary.Help)
        {
            await HelpInfo.SendAsync(user, answerDictionary, cancellationToken);
        }
        else if (message.Text.StartsWith(commandDictionary.ComicByDate))
        {
            string textDate = message.Text[commandDictionary.ComicByDate.Length..];
            var date = DateUtils.TryParseDate(textDate, user.Language);

            if (date is not null)
            {
                await ComicByDate.SendAsync(date.Value, user, answerDictionary, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                _messagesSenderService.EnqueueMessage(new TextMessage(user.Id, answerDictionary.WrongDateFormat));
            }
        }
        else if (message.Text.StartsWith(commandDictionary.SetDate))
        {
            string textDate = message.Text[commandDictionary.SetDate.Length..];
            var date = DateUtils.TryParseDate(textDate, user.Language);

            if (date is not null)
            {
                await SetDate.ExecuteAsync(date.Value, user, cancellationToken);
                await repository.UpdateAsync(user, cancellationToken);
            }
            else
            {
                _messagesSenderService.EnqueueMessage(new TextMessage(user.Id, answerDictionary.WrongDateFormat));
            }
        }
        else
        {
            AntiSpamService.UserRequestProcessed(userId);
        }
    }
}
