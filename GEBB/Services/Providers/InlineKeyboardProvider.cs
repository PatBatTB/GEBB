using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineKeyboardProvider
{
    private static readonly Dictionary<CallbackMenu, Func<InlineKeyboardMarkup>> InlineReplyMarkupDict = new()
    {
        [CallbackMenu.Main] = GetMainMarkup,
        [CallbackMenu.MyEvents] = GetMyEventsMarkup,
        [CallbackMenu.CreateEvent] = GetCreateEventMarkup
    };

    public static InlineKeyboardMarkup GetMarkup(CallbackMenu callbackMenu)
    {
        return InlineReplyMarkupDict.GetValueOrDefault(callbackMenu, UnknownMarkup).Invoke();
    }

    private static InlineKeyboardMarkup GetMainMarkup()
    {
        var myEvents = InlineButtonProvider.GetButton(CallbackMenu.Main, CallbackButton.MyEvents);
        var myRegs = InlineButtonProvider.GetButton(CallbackMenu.Main, CallbackButton.MyRegistrations);
        var availEvents =
            InlineButtonProvider.GetButton(CallbackMenu.Main, CallbackButton.AvailableEvents);
        var close = InlineButtonProvider.GetButton(CallbackMenu.Main, CallbackButton.Close);
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
        var create = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallbackButton.Create);
        var list = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallbackButton.List);
        var back = InlineButtonProvider.GetButton(CallbackMenu.MyEvents, CallbackButton.Back);
        return new InlineKeyboardMarkup(
            [
                [create, list],
                [back]
            ]
        );
    }

    private static InlineKeyboardMarkup GetCreateEventMarkup()
    {
        var title = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Title);
        var dateTimeOf = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.DateTimeOf);
        var address = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Address);
        var cost = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Cost);
        var participantLimit =
            InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.ParticipantLimit);
        var description = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Description);
        var done = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Done);
        var close = InlineButtonProvider.GetButton(CallbackMenu.CreateEvent, CallbackButton.Close);
        return new InlineKeyboardMarkup(
            [
                [title],
                [dateTimeOf],
                [address],
                [cost],
                [participantLimit],
                [description],
                [done],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup UnknownMarkup()
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }
}