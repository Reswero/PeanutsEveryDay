namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public abstract class AbstractMessage
{
    public long UserId { get; private set; }

    public AbstractMessage(long userId)
    {
        UserId = userId;
    }
}
