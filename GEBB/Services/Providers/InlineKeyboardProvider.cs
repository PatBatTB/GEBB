using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
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

    private static readonly Dictionary<CallbackMenu, Func<CallbackMenu, string, InlineKeyboardMarkup>>
        InlineReplyMarkupWithIdDict = new()
            {
                [CallbackMenu.CreatedEvent] = GetEventHandleMarkup,
                [CallbackMenu.RegEventDescr] = GetRegisteredEventMarkup,
                [CallbackMenu.RegEventPart] = GetAlterRegisteredEventMarkup,
            };

    public static InlineKeyboardMarkup GetMarkup(CallbackMenu menu)
    {
        return InlineReplyMarkupDict.GetValueOrDefault(menu, UnknownMarkup).Invoke(menu);
    }

    public static InlineKeyboardMarkup GetMarkup(CallbackMenu menu, string eventId)
    {
        return InlineReplyMarkupWithIdDict.GetValueOrDefault(menu, UnknownMarkup).Invoke(menu, eventId);
    }

    public static InlineKeyboardMarkup GetDynamicCreateEventMarkup(EventDto entity)
    {
        var menu = CallbackMenu.CreateEvent;
        var markup = GetMarkup(CallbackMenu.CreateEvent);

        Dictionary<string, InlineKeyboardButton> buttonsMappingDict = new();
        if (entity.Title is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.Title, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.TitleDone, Menu = menu })
            );
        if (entity.DateTimeOf is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.DateTimeOf, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.DateTimeOfDone, Menu = menu })
            );
        if (entity.Address is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.Address, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.AddressDone, Menu = menu })
            );
        if (entity.ParticipantLimit is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.ParticipantLimit, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(new CallbackData
                    { Button = CallbackButton.ParticipantLimitDone, Menu = menu })
            );
        if (entity.Cost is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.Cost, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.CostDone, Menu = menu })
            );
        if (entity.Description is not null)
            buttonsMappingDict.Add(
                new CallbackData() { Button = CallbackButton.Description, Menu = menu }.GetDataString(),
                InlineButtonProvider.GetButton(
                    new CallbackData { Button = CallbackButton.DescriptionDone, Menu = menu })
            );

        var newKeyboard = markup.InlineKeyboard.Select(row =>
            row.Select(button =>
                buttonsMappingDict!.GetValueOrDefault(button.CallbackData, button))).ToList();
        return new InlineKeyboardMarkup(newKeyboard);
    }

    private static InlineKeyboardMarkup GetMainMarkup(CallbackMenu menu)
    {
        var myEvents = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.MyEvents, Menu = menu });
        var myRegs = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.MyRegistrations, Menu = menu });
        var availEvents = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.AvailableEvents, Menu = menu });
        var close = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Close, Menu = menu });
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
        var create = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Create, Menu = menu });
        var list = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.List, Menu = menu });
        var back = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Back, Menu = menu });
        return new InlineKeyboardMarkup(
            [
                [create, list],
                [back]
            ]
        );
    }

    private static InlineKeyboardMarkup GetCreateEventMarkup(CallbackMenu menu)
    {
        var title = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Title, Menu = menu });
        var dateTimeOf = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.DateTimeOf, Menu = menu });
        var address = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Address, Menu = menu });
        var cost = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Cost, Menu = menu });
        var participantLimit = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.ParticipantLimit, Menu = menu });
        var description = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Description, Menu = menu });
        var finish = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.FinishCreating, Menu = menu });
        var close = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Close, Menu = menu });
        return new InlineKeyboardMarkup(
            [
                [title],
                [dateTimeOf],
                [address],
                [cost],
                [participantLimit],
                [description],
                [finish],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup GetYesNoMarkup(CallbackMenu menu)
    {
        var yes = InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.Yes, Menu = menu });
        var no = InlineButtonProvider.GetButton(new CallbackData { Button = CallbackButton.No, Menu = menu });
        return new InlineKeyboardMarkup(
            [
                [yes, no]
            ]
        );
    }

    private static InlineKeyboardMarkup GetEventHandleMarkup(CallbackMenu menu, string eventId)
    {
        var edit = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Edit, Menu = menu, EventId = eventId});
        var cancel = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Cancel, Menu = menu, EventId = eventId});
        var close = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Close, Menu = menu, EventId = eventId});
        return new InlineKeyboardMarkup(
            [
                [edit, cancel],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup GetRegisteredEventMarkup(CallbackMenu menu, string eventId)
    {
        var participantList = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.ParticipantList, Menu = menu, EventId = eventId });
        var cancel = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.CancelRegistration, Menu = menu, EventId = eventId});
        var close = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Close, Menu = menu, EventId = eventId });
        return new InlineKeyboardMarkup(
            [
                [participantList],
                [cancel],
                [close]
            ]
        );
    }

    private static InlineKeyboardMarkup GetAlterRegisteredEventMarkup(CallbackMenu menu, string eventId)
    {
        var toDescription = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.ToDescription, Menu = menu, EventId = eventId });
        var cancel = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.CancelRegistration, Menu = menu, EventId = eventId });
        var close = InlineButtonProvider
            .GetButton(new CallbackData { Button = CallbackButton.Close, Menu = menu, EventId = eventId});
        return new InlineKeyboardMarkup(
            [
                [toDescription],
                [cancel],
                [close]
            ]
        );
    }

    public static InlineKeyboardMarkup RegistrationMarkup(CallbackData data)
    {
        return new InlineKeyboardMarkup([[InlineButtonProvider.GetButton(data)]]);
    }

    private static InlineKeyboardMarkup UnknownMarkup(CallbackMenu menu)
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }

    private static InlineKeyboardMarkup UnknownMarkup(CallbackMenu menu, string eventId)
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }
}