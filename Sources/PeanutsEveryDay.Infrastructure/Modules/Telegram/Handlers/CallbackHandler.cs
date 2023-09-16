using Microsoft.Extensions.DependencyInjection;
using PeanutsEveryDay.Application.Modules.Repositories;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;
using Telegram.Bot.Types;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Handlers;

public static class CallbackHandler
{
    private static IServiceProvider _serviceProvider;
    private static MessagesSenderService _messagesSenderService;

    public static void Init(IServiceProvider serviceProvider, MessagesSenderService messagesSenderService)
    {
        _serviceProvider = serviceProvider;
        _messagesSenderService = messagesSenderService;
    }

    public static async Task HandleAsync(CallbackQuery callback, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

        long chatId = callback.From.Id;
        var user = (await repository.GetAsync(chatId, cancellationToken))!;

        var dictionary = scope.ServiceProvider.GetRequiredService<CallbackDictionaryResolver>().Invoke(user.Language);

        if (callback.Data is null)
            return;

        if (callback.Data.StartsWith(CallbackKey.SourcePrefix))
        {
            CallbackHelper.ChangeSources(callback.Data, user.Settings);
            await repository.UpdateAsync(user, cancellationToken);
            callback.Data = CallbackKey.Sources;
        }
        else if (callback.Data.StartsWith(CallbackKey.PeriodPrefix))
        {
            CallbackHelper.ChangePeriod(callback.Data, user.Settings);
            await repository.UpdateAsync(user, cancellationToken);
            callback.Data = CallbackKey.Period;
        }

        var (template, inlineKeyboard) = CallbackHelper.GetTemplateWithKeyboardMarkup(callback.Data, user, dictionary);

        int messageId = callback.Message!.MessageId;
        if (template is not null && inlineKeyboard is not null)
        {
            _messagesSenderService.EnqueueMessage(new EditMessage(user.Id, messageId, template, inlineKeyboard));
        }
        else
        {
            _messagesSenderService.EnqueueMessage(new DeleteMessage(user.Id, messageId));
        }
    }
}
