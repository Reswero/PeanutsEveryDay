namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;

public static class AnswerDictionary
{
    public const string NoComicByDate = "Нет комикса по указанной дате :(";
    public const string Greetings = "Привет! :)";
    public const string ComicsOut = "Комиксы закончились :(";
    public const string NeededAtLeastOneSource = "Должен быть выбран хотя бы один источник с комиксами!";
    public const string WrongDateFormat =
        """
        Указан неверный формат даты
        Пример формата: 31.12.2000
        """;
}
