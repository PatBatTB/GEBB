using log4net;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Com.GitHub.PatBatTB.GEBB.Domain;

internal static class BotConfig
{
    private static ILog Log = LogManager.GetLogger(typeof(BotConfig));
    
    static BotConfig()
    {
        if (Environment.GetEnvironmentVariable("bot.token") is { } token) 
            BotToken = token;
        else
        {
            Log.Fatal("Env variable 'bot.token' not found.");
            throw new NullReferenceException();
        }

        ReceiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
                UpdateType.Message,
                UpdateType.CallbackQuery
            ],
            DropPendingUpdates = false
        };
    }

    internal static string BotToken { get; }
    internal static ReceiverOptions ReceiverOptions { get; }
}