using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Com.GitHub.PatBatTB.GEBB.Domain;

internal static class BotConfig
{
    static BotConfig()
    {
        if (Environment.GetEnvironmentVariable("bot.token") is { } token)
            BotToken = token;
        else
            throw new NullReferenceException("env var 'bot.token' must be token of your bot");

        ReceiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
                UpdateType.Message,
                UpdateType.CallbackQuery
            ],
            DropPendingUpdates = true
        };
    }

    internal static string BotToken { get; }
    internal static ReceiverOptions ReceiverOptions { get; }
}