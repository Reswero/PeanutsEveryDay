using System.Collections.Concurrent;

namespace PeanutsEveryDay.Infrastructure.Modules.Telegram.Services;

public static class AntiSpamService
{
    private static readonly ConcurrentDictionary<long, bool> _userRequestStates = new();

    public static bool IsUserRequestInProcess(long userId)
    {
        return _userRequestStates.GetOrAdd(userId, false);
    }

    public static void ProcessUserRequest(long userId)
    {
        _userRequestStates[userId] = true;
    }

    public static void UserRequestProcessed(long userId)
    {
        _userRequestStates[userId] = false;
    }
}
