using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Ru;

public class RuAnswerDictionary : AnswerDictionary
{
    public override string Greetings { get; } =
        """
        Привет!
        Я бот, который будет присылать тебе комиксы про Чарли Брауна и его друзей.

        На русский язык переведены не все комиксы, поэтому начиная с определенной даты комиксы будут на английском языке

        Для получения информации по доступным командам используй:
        /help
        """;
    public override string HelpInformation { get; } =
        """
        Доступные команды:

        /help - информация о всех командах
        /keyboard - установка клавиатуры
        /menu - меню настроек
        /next - получить следующий комикс
        /date 31.12.2000 - получение комикса по дате
        /setdate 31.12.2000 - установка даты с которой продолжится получение комиксов
        /stop - остановить рассылку комиксов
        """;
    public override string KeyboardInstalled { get; } = "Клавиатура установлена!";
    public override string NoComicByDate { get; } = "Нет комикса по указанной дате :(";
    public override string ComicsOut { get; } = "Комиксы закончились :(";
    public override string SendingStopped { get; } = "Рассылка комиксов остановлена!";
    public override string NeededAtLeastOneSource { get; } = "Должен быть выбран хотя бы один источник с комиксами!";
    public override string WrongDateFormat { get; } =
        """
        Указан неверный формат даты
        Пример формата: 31.12.2000
        """;
}
