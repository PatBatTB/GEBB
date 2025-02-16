using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class BotCommandProvider
{
    public static List<BotCommand> GetCommandMenu(UserStatus status)
    {
        return Enum.GetValues<Command>()
            .Where(elem => elem.Scope().Contains(status))
            .Select(elem => new BotCommand { Command = elem.Name(), Description = elem.Description() })
            .ToList();
    }
}