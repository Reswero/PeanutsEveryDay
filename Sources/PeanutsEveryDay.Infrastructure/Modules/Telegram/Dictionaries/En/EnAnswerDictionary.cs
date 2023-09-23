using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.En;

public class EnAnswerDictionary : AnswerDictionary
{
    public override string Greetings { get; } =
        """
        Hi!
        I am a bot that will send you comics about Charlie Brown and his friends.

        For information on available commands use:
        /help
        """;
    public override string HelpInformation { get; } =
        """
        Available commands:

        /help - information about all commands
        /keyboard - keyboard setup
        /menu - settings menu
        /next - get next comic
        /date 12/31/2000 - get comic by date
        /setdate 12/31/2000 - set the date from which the comics will continue to be received
        /stop - stop sending comics
        """;
    public override string KeyboardInstalled { get; } = "Keyboard installed!";
    public override string NoComicByDate { get; } = "There is no comic for the date listed :(";
    public override string ComicsOut { get; } = "Comics are out :(";
    public override string SendingStopped { get; } = "Sending of comics has been stopped!";
    public override string NeededAtLeastOneSource { get; } = "At least one comic source must be selected!";
    public override string WrongDateFormat { get; } =
        """
        Incorrect date format is specified
        Format example: 12/31/2000
        """;
}
