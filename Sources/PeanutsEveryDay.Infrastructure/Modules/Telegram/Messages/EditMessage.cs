using Telegram.Bot.Types.ReplyMarkups;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class EditMessage : TextMessage
{
    public int MessageId { get; private set; }

    public EditMessage(long userId, int messageId, string text, IReplyMarkup? replyMarkup = null) :
        base(userId, text, replyMarkup)
    {
        MessageId = messageId;
    }
}
