using Com.Github.PatBatTB.GEBB.DataBase.Entity;
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
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.TitleDone, Menu = menu})
            );
        if (entity.DateTimeOf is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.DateTimeOf),
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.DateTimeOfDone, Menu = menu})
            );
        if (entity.Address is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Address),
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.AddressDone, Menu = menu})
            );
        if (entity.ParticipantLimit is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.ParticipantLimit),
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.ParticipantLimitDone, Menu = menu})
            );
        if (entity.Cost is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Cost),
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.CostDone, Menu = menu})
            );
        if (entity.Description is not null)
            buttonsMappingDict.Add(
                CallbackData.GetDataString(menu, CallbackButton.Description),
                InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.DescriptionDone, Menu = menu})
            );

        var newKeyboard = markup.InlineKeyboard.Select(row =>
            row.Select(button =>
                buttonsMappingDict!.GetValueOrDefault(button.CallbackData, button))).ToList();
        return new InlineKeyboardMarkup(newKeyboard);
    }

    private static InlineKeyboardMarkup GetMainMarkup(CallbackMenu menu)
    {
        var myEvents = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.MyEvents, Menu = menu});
        var myRegs = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.MyRegistrations, Menu = menu});
        var availEvents = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.AvailableEvents, Menu = menu});
        var close = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Close, Menu = menu});
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
            .GetButton(new AlterCbData { Button = CallbackButton.Create, Menu = menu});
        var list = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.List, Menu = menu});
        var back = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Back, Menu = menu});
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
            .GetButton(new AlterCbData { Button = CallbackButton.Title, Menu = menu});
        var dateTimeOf = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.DateTimeOf, Menu = menu});
        var address = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Address, Menu = menu});
        var cost = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Cost, Menu = menu});
        var participantLimit = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.ParticipantLimit, Menu = menu});
        var description = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Description, Menu = menu});
        var finish = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.FinishCreating, Menu = menu});
        var close = InlineButtonProvider
            .GetButton(new AlterCbData { Button = CallbackButton.Close, Menu = menu});
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
        var yes = InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.Yes, Menu = menu});
        var no = InlineButtonProvider.GetButton(new AlterCbData { Button = CallbackButton.No, Menu = menu});
        return new InlineKeyboardMarkup(
            [
                [yes, no]
            ]
        );
    }

    public static InlineKeyboardMarkup RegistrationMarkup(AlterCbData callbackData)
    {
        return new InlineKeyboardMarkup([[InlineButtonProvider.GetButton(callbackData)]]);
    }

    private static InlineKeyboardMarkup UnknownMarkup(CallbackMenu menu)
    {
        throw new ArgumentException("Unknown CallbackMenu");
    }
}