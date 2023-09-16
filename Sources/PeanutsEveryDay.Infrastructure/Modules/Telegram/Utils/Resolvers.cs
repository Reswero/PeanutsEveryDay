using PeanutsEveryDay.Abstraction;
using PeanutsEveryDay.Infrastructure.Modules.Telegram.Dictionaries.Abstractions;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Utils;

public delegate CommandDictionary CommandDictionaryResolver(LanguageCode language);

public delegate AnswerDictionary AnswerDictionaryResolver(LanguageCode language);

public delegate CallbackDictionary CallbackDictionaryResolver(LanguageCode language);
