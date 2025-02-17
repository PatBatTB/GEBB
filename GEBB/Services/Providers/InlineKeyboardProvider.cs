using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineKeyboardProvider
{
    private static readonly Dictionary<CallbackMenu, Func<InlineKeyboardMarkup>> InlineReplyMarkupDict = new()
    {
        [CallbackMenu.Main] = GetMainMarkup,
        [CallbackMenu.MyEvents] = GetMyEventsMarkup
    };

    public static InlineKeyboardMarkup GetMarkup(CallbackMenu callbackMenu)
    {
        return InlineReplyMarkupDict.GetValueOrDefault(callbackMenu, UnknownMarkup).Invoke();
    }

    private static InlineKeyboardMarkup GetMainMarkup()
    {
        var myEvents = InlineButtonProvider.GetButton(CallbackMenu.Main, CallBackButton.MyEvents);
        var myRegs = InlineButtonProvider.GetButton(CallbackMenu.Main, CallBackButton.MyRegistrations);
        var availEvents =
            InlineButtonProvider.GetButton(CallbackMenu.Main, CallBackButton.AvailableEvents);
        var close = InlineButtonProvider.GetButton(CallbackMenu.Main, CallBackButton.Close);
        return new InlineKeyboardMarkup(
            [
                [myEvents],
                [myRegs, availEvents],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup GetMyEventsMarkup()
    {
        var create = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallBackButton.Create);
        var list = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallBackButton.List);
        var back = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallBackButton.Back);
        return new InlineKeyboardMarkup(
            [
                [create, list],
                [back]
            ]
        );
    }

    private static InlineKeyboardMarkup UnknownMarkup()
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }
}