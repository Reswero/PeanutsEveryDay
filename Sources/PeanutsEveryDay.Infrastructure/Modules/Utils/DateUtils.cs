using PeanutsEveryDay.Abstraction;
using System.Globalization;

namespace PeanutsEveryDay.Infrastructure.Modules.Utils;

public static class DateUtils
{
    private static readonly CultureInfo _ruCulture = new("ru-RU");
    private static readonly CultureInfo _enCulture = new("en-US");

    public static string ConvertDate(DateOnly date, LanguageCode language)
    {
        DateTime dt = new(date.Year, date.Month, date.Day);

        if (language == LanguageCode.Ru)
        {
            return dt.ToString("dd MMMM yyyy", _ruCulture);
        }
        else
        {
            return dt.ToString("MMMM dd, yyyy", _enCulture);
        }
    }

    public static DateOnly? TryParseDate(string date, LanguageCode language)
    {
        bool parsed = false;
        DateOnly dateOnly;

        if (language == LanguageCode.Ru)
        {
            parsed = DateOnly.TryParse(date, _ruCulture, out dateOnly);
        }
        else
        {
            parsed = DateOnly.TryParse(date, _enCulture, out dateOnly);
        }

        return parsed ? dateOnly : null;
    }
}
