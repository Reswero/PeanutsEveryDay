using PeanutsEveryDay.Domain.Models;
using System.Diagnostics.CodeAnalysis;

namespace PeanutsEveryDay.Application.Modules.Comics;

public class ComicsComparer : IEqualityComparer<Comic>
{
    public bool Equals(Comic? x, Comic? y)
    {
        return (x?.PublicationDate == y?.PublicationDate) &&
            (x?.Source == y?.Source);
    }

    public int GetHashCode([DisallowNull] Comic obj)
    {
        return obj.PublicationDate.GetHashCode() ^ obj.Source.GetHashCode();
    }
}
