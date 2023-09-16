using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Application.Modules.Parsers.Models;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Messages;

public class ComicMessage : AbstractMessage
{
    public ComicMessage(long userId, LanguageCode language, ParsedComic comic) :
        base(userId)
    {
        Language = language;
        Comic = comic;
    }
    
    public LanguageCode Language { get; private set;}
    public ParsedComic Comic { get; private set; }
}
