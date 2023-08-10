namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class DeleteMessage : AbstractMessage
{
    public DeleteMessage(long userId, int messageId) : base(userId)
    {
        MessageId = messageId;
    }

    public int MessageId { get; private set; }
}
