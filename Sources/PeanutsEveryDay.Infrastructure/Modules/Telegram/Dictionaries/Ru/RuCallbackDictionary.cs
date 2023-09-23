using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Ru;

public class RuCallbackDictionary : CallbackDictionary
{
    public override string MainMenu => "Меню 📋";
    public override string Progress => "Прогресс 📈";
    public override string Settings => "Настройки ⚙";
    public override string Hide => "Скрыть ❎";
    public override string Back => "Назад ↩";

    public override string ProgressTemplate =>
            """
            Прогресс 📈

            Всего просмотрено комиксов: {0}
            Текущая дата: {1}
            Просмотрено {2} комиксов из ~{3}
            """;

    public override string Sources => "Источники 👀";
    public override string SourcesInfo => "Информация ℹ";
    public override string SourcesInfoTemplate =>
        """
        Информация ℹ

        [Acomics](https://acomics.ru/~peanuts) - оригинальные переведенные комиксы
        [Acomics Begins](https://acomics.ru/~peanutsbegins) - переведенный цветной перезапуск первых комиксов
        [Gocomics](https://www.gocomics.com/peanuts) - оригинальные комиксы на английском языке
        [Gocomics Begins](https://www.gocomics.com/peanuts-begins) - цветной перезапуск первых комиксов на английском языке

        Не все оригинальные комиксы имеют цветные версии.
        Если стиль или язык комиксов изменился, значит в источнике закончились комиксы.

        [Авторы русского перевода](https://vk.com/ruspeanuts)
        """;

    public override string SendingPeriod => "Период рассылки 🕒";
    public override string EveryHour => "Каждый час";
    public override string EveryDay => "Каждый день";
}
