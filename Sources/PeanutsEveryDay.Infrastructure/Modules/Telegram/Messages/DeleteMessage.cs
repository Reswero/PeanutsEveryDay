namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class DeleteMessage : AbstractMessage
{
    public int MessageId { get; private set; }

    public DeleteMessage(long userId, int messageId) : base(userId)
    {
        MessageId = messageId;
    }
}
