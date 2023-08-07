using PeanutsEveryDay.Application.Modules.Parsers.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class ComicMessage : AbstractMessage
{
    public ComicMessage(long userId, ParsedComic comic) :
        base(userId)
    {
        Comic = comic;
    }
    
    public ParsedComic Comic { get; private set; }
}
