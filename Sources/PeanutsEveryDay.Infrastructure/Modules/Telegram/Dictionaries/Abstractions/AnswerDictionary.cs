namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

public abstract class AnswerDictionary
{
    public abstract string NoComicByDate { get; }
    public abstract string Greetings { get; }
    public abstract string ComicsOut { get; }
    public abstract string NeededAtLeastOneSource { get; }
    public abstract string WrongDateFormat { get; }
}