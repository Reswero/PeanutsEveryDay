using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries;

public class RuCallbackDictionary : CallbackDictionary
{
    public override string MainMenu => "Меню";
    public override string Progress => "Прогресс";
    public override string Settings => "Настройки";
    public override string Hide => "Скрыть";
    public override string Back => "Назад";

    public override string ProgressTemplate =>
            """
            Прогресс

            Всего просмотрено комиксов: {0}
            Текущая дата: {1}
            Просмотрено {2} комиксов из ~{3}
            """;

    public override string Sources => "Источники";
    public override string Period => "Период";
    public override string EveryHour => "Каждый час";
    public override string EveryDay => "Каждый день";
}
