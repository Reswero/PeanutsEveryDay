using Microsoft.Extensions.Logging;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;
using PeanutsEveryDay.Infrastructure.Modules.Utils;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Timer = System.Timers.Timer;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

public class MessagesSenderService
{
    private const int _maxTasks = 4;
    private const int _maxMessagesPerSending = 16;

    private readonly ConcurrentQueue<AbstractMessage> _messages = new();

    private readonly TimeSpan _sendingInterval = TimeSpan.FromSeconds(0.5);
    private readonly Timer _timer;

    private readonly ILogger<MessagesSenderService> _logger;
    private readonly ITelegramBotClient _bot;

    public MessagesSenderService(ILogger<MessagesSenderService> logger, ITelegramBotClient bot)
    {
        _logger = logger;
        _bot = bot;

        _timer = new()
        {
            AutoReset = true,
            Enabled = true,
            Interval = _sendingInterval.TotalMilliseconds
        };
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();
    }

    public void EnqueueMessage(AbstractMessage message)
    {
        _messages.Enqueue(message);
    }

    private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_messages.IsEmpty)
            return;

        ConcurrentQueue<AbstractMessage> messagesToSend = _messages;
        while (!_messages.IsEmpty && messagesToSend.Count < _maxMessagesPerSending &&
            _messages.TryDequeue(out AbstractMessage message))
        {
            messagesToSend.Enqueue(message);
        }

        Task[] tasks = new Task[_maxTasks];
        for (int i = 0; i < _maxTasks; i++)
        {
            var task = Task.Run(async () => await SendMessagesFromQueue(messagesToSend));
            tasks[i] = task;
        }

        await Task.WhenAll(tasks);
    }

    private async Task SendMessagesFromQueue(ConcurrentQueue<AbstractMessage> queue)
    {
        while (!queue.IsEmpty && queue.TryDequeue(out AbstractMessage message))
        {
            try
            {
                await SendMessage(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured while sending message. {Error}", ex.Message);
            }
            finally
            {
                AntiSpamService.UserRequestProcessed(message.UserId);
            }
        }
    }

    private async Task SendMessage(AbstractMessage message)
    {
        switch (message)
        {
            case ComicMessage c:
                string date = DateUtils.ConvertDate(c.Comic.PublicationDate, c.Language);
                string caption = $"[{date}]({c.Comic.Url})";
                InputFileStream inputFile = new(c.Comic.ImageStream, c.Comic.PublicationDate.ToShortDateString());
                await _bot.SendPhotoAsync(c.UserId, inputFile, caption: caption, parseMode: ParseMode.Markdown);
                break;
            case EditMessage e:
                await _bot.EditMessageTextAsync(e.UserId, e.MessageId, e.Text, replyMarkup: e.ReplyMarkup as InlineKeyboardMarkup);
                break;
            case DeleteMessage d:
                await _bot.DeleteMessageAsync(d.UserId, d.MessageId);
                break;
            case TextMessage t:
                await _bot.SendTextMessageAsync(t.UserId, t.Text, replyMarkup: t.ReplyMarkup);
                break;
            case CommandListMessage l:
                await _bot.SetMyCommandsAsync(l.Commands, l.CommandsScope);
                break;
        }
    }
}
