namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public abstract class AbstractMessage
{
    public AbstractMessage(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; private set; }
}
