﻿using PeanutsEveryDay.Domain.Models;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Commands;

public static class KeyboardMenu
{
    private static MessagesSenderService _senderService;

    public static void Init(MessagesSenderService senderService)
    {
        _senderService = senderService;
    }

    public static async Task SendAsync(User user, CommandDictionary commandDictionary,
        AnswerDictionary answerDictionary, CancellationToken cancellationToken)
    {
        ReplyKeyboardMarkup replyKeyboard = new(new[]
        {
            new KeyboardButton(commandDictionary.NextComic),
            new KeyboardButton(commandDictionary.Menu)
        })
        {
            ResizeKeyboard = true
        };

        _senderService.EnqueueMessage(new TextMessage(user.Id, answerDictionary.Greetings, replyKeyboard));
    }
}
