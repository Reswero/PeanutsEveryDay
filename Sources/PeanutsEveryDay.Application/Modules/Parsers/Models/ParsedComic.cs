using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Application.Modules.Parsers.Models;

public record ParsedComic
{
    public ParsedComic(DateOnly publicationDate, SourceType source, string url, Stream imageStream)
    {
        PublicationDate = publicationDate;
        Source = source;
        Url = url;
        ImageStream = new();

        imageStream.CopyTo(ImageStream);
        ImageStream.Seek(0, SeekOrigin.Begin);
    }

    public DateOnly PublicationDate { get; set; }
    public SourceType Source { get; set; }
    public string Url { get; set; }
    public MemoryStream ImageStream { get; set; }
}
