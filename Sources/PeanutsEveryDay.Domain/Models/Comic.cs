using PeanutsEveryDay.Abstraction;

namespace PeanutsEveryDay.Domain.Models;

public class Comic
{
    public required DateOnly PublicationDate { get; init; }
    public required SourceType Source { get; init; }
    public required string Url { get; init; }
}
