using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineButtonProvider
{
    public static InlineKeyboardButton GetButton(CallbackMenu callbackMenu, CallBackButton callBackButton)
    {
        return InlineKeyboardButton.WithCallbackData(
            callBackButton.Text(),
            CallbackData.GetDataString(callbackMenu, callBackButton));
    }
}