using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Ru;

public class RuAnswerDictionary : AnswerDictionary
{
    public override string NoComicByDate { get; } = "Нет комикса по указанной дате :(";
    public override string Greetings { get; } = "Привет! :)";
    public override string ComicsOut { get; } = "Комиксы закончились :(";
    public override string NeededAtLeastOneSource { get; } = "Должен быть выбран хотя бы один источник с комиксами!";
    public override string WrongDateFormat { get; } =
        """
        Указан неверный формат даты
        Пример формата: 31.12.2000
        """;
}
