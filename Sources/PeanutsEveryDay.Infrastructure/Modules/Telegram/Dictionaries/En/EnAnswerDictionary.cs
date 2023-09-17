using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.En;

public class EnAnswerDictionary : AnswerDictionary
{
    public override string NoComicByDate { get; } = "There is no comic for the date listed :(";
    public override string Greetings { get; } = "Hello! :)";
    public override string ComicsOut { get; } = "Comics are out :(";
    public override string NeededAtLeastOneSource { get; } = "At least one comic source must be selected!";
    public override string WrongDateFormat { get; } =
        """
        Incorrect date format is specified
        Format example: 12/31/2000
        """;
}
