using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineButtonProvider
{
    public static InlineKeyboardButton GetButton(CallbackMenu callbackMenu, CallbackButton callbackButton)
    {
        return InlineKeyboardButton.WithCallbackData(
            callbackButton.Text(),
            CallbackData.GetDataString(callbackMenu, callbackButton));
    }
}