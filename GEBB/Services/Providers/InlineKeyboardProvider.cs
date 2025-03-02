using Com.Github.PatBatTB.GEBB.DataBase.Entity;
using Com.Github.PatBatTB.GEBB.Domain;
using Telegram.Bot.Types.ReplyMarkups;

namespace Com.Github.PatBatTB.GEBB.Services.Providers;

public static class InlineKeyboardProvider
{
    private static readonly Dictionary<CallbackMenu, Func<CallbackMenu, InlineKeyboardMarkup>> InlineReplyMarkupDict =
        new()
        {
            [CallbackMenu.Main] = GetMainMarkup,
            [CallbackMenu.MyEvents] = GetMyEventsMarkup,
            [CallbackMenu.CreateEvent] = GetCreateEventMarkup,
            [CallbackMenu.EventTitleReplace] = GetYesNoMarkup,
            [CallbackMenu.EventDateTimeOfAgain] = GetYesNoMarkup,
            [CallbackMenu.EventDateTimeOfReplace] = GetYesNoMarkup,
            [CallbackMenu.EventAddressReplace] = GetYesNoMarkup,
            [CallbackMenu.EventCostReplace] = GetYesNoMarkup,
            [CallbackMenu.EventParticipantLimitReplace] = GetYesNoMarkup,
            [CallbackMenu.EventDescriptionReplace] = GetYesNoMarkup,
        };

    public static InlineKeyboardMarkup GetMarkup(CallbackMenu callbackMenu)
    {
        return InlineReplyMarkupDict.GetValueOrDefault(callbackMenu, UnknownMarkup).Invoke(callbackMenu);
    }

    public static InlineKeyboardMarkup GetDynamicCreateEventMarkup(EventEntity entity)
    {
        var menu = CallbackMenu.CreateEvent;
        var markup = GetMarkup(CallbackMenu.CreateEvent);

        Dictionary<string, InlineKeyboardButton> buttonsMappingDict = new();
        if (entity.Title is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Title),
                InlineButtonProvider.GetButton(menu, CallbackButton.TitleDone)
            );

        if (entity.DateTimeOf is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.DateTimeOf),
                InlineButtonProvider.GetButton(menu, CallbackButton.DateTimeOfDone)
            );

        if (entity.Address is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Address),
                InlineButtonProvider.GetButton(menu, CallbackButton.AddressDone)
            );

        if (entity.ParticipantLimit is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.ParticipantLimit),
                InlineButtonProvider.GetButton(menu, CallbackButton.ParticipantLimitDone)
            );

        if (entity.Cost is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Cost),
                InlineButtonProvider.GetButton(menu, CallbackButton.CostDone)
            );

        if (entity.Description is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Description),
                InlineButtonProvider.GetButton(menu, CallbackButton.DescriptionDone));

        var newKeyboard = markup.InlineKeyboard.Select(row =>
            row.Select(button =>
                buttonsMappingDict!.GetValueOrDefault(button.CallbackData, button))).ToList();
        return new InlineKeyboardMarkup(newKeyboard);
    }

    private static InlineKeyboardMarkup GetMainMarkup(CallbackMenu callbackMenu)
    {
        var menu = CallbackMenu.Main;
        var myEvents = InlineButtonProvider.GetButton(menu, CallbackButton.MyEvents);
        var myRegs = InlineButtonProvider.GetButton(menu, CallbackButton.MyRegistrations);
        var availEvents =
            InlineButtonProvider.GetButton(menu, CallbackButton.AvailableEvents);
        var close = InlineButtonProvider.GetButton(menu, CallbackButton.Close);
        return new InlineKeyboardMarkup(
            [
                [myEvents],
                [myRegs, availEvents],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup GetMyEventsMarkup(CallbackMenu menu)
    {
        var create = InlineButtonProvider.GetButton(menu, CallbackButton.Create);
        var list = InlineButtonProvider.GetButton(menu, CallbackButton.List);
        var back = InlineButtonProvider.GetButton(menu, CallbackButton.Back);
        return new InlineKeyboardMarkup(
            [
                [create, list],
                [back]
            ]
        );
    }

    private static InlineKeyboardMarkup GetCreateEventMarkup(CallbackMenu menu)
    {
        var title = InlineButtonProvider.GetButton(menu, CallbackButton.Title);
        var dateTimeOf = InlineButtonProvider.GetButton(menu, CallbackButton.DateTimeOf);
        var address = InlineButtonProvider.GetButton(menu, CallbackButton.Address);
        var cost = InlineButtonProvider.GetButton(menu, CallbackButton.Cost);
        var participantLimit = InlineButtonProvider.GetButton(menu, CallbackButton.ParticipantLimit);
        var description = InlineButtonProvider.GetButton(menu, CallbackButton.Description);
        var done = InlineButtonProvider.GetButton(menu, CallbackButton.FinishCreating);
        var close = InlineButtonProvider.GetButton(menu, CallbackButton.Close);
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

    private static InlineKeyboardMarkup GetYesNoMarkup(CallbackMenu menu)
    {
        var yes = InlineButtonProvider.GetButton(menu, CallbackButton.Yes);
        var no = InlineButtonProvider.GetButton(menu, CallbackButton.No);
        return new InlineKeyboardMarkup(
            [
                [yes, no]
            ]
        );
    }

    private static InlineKeyboardMarkup UnknownMarkup(CallbackMenu menu)
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }
}