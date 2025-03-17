using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineButtonProvider
{
    public static InlineKeyboardButton GetButton(AlterCbData callbackData)
    {
        if (callbackData.Button is not { } button) throw new ArgumentException("CallbackData doesn't have button");
        if (callbackData.Menu is not { } menu) throw new ArgumentException("callbackData doesn't have menu");
        return InlineKeyboardButton.WithCallbackData(
            button.Text(),
            callbackData.GetDataString());
    }
}