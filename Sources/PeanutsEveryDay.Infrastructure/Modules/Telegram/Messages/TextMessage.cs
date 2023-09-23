using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class TextMessage : AbstractMessage
{
    public string Text { get; private set; }
    public IReplyMarkup? ReplyMarkup { get; private set; }

    public TextMessage(long userId, string text, IReplyMarkup? replyMarkup = null) :
        base(userId)
    {
        Text = text;
        ReplyMarkup = replyMarkup;
    }
}
