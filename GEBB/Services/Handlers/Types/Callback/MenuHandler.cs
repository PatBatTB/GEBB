using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback;

public static class MenuHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> MenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainHandler.Handle,
        [CallbackMenu.CreateEvent] = BuildEventHandler.Handle,
        [CallbackMenu.EventsList] = EventsListHandler.Handle,
        [CallbackMenu.EditEvent] = BuildEventHandler.Handle,
        [CallbackMenu.EventTitleReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfAgain] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventAddressReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventCostReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventPartLimitReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDescrReplace] = HandleEventReplaceMenu,
        [CallbackMenu.RegisterToEvent] = HandleEventRegisterMenu,
        [CallbackMenu.CreatedEvent] = IndividualEventHandler.HandleMyOwn,
        [CallbackMenu.CreEventPart] = IndividualEventHandler.HandleMyOwnPart,
        [CallbackMenu.RegEventDescr] = IndividualEventHandler.HandleRegisteredDescr,
        [CallbackMenu.RegEventPart] = IndividualEventHandler.HandleRegisteredPart,
        [CallbackMenu.Settings] = SettingsHandler.Handle,
        [CallbackMenu.Alarm] = SettingsHandler.HandleAlarmMenu,
        [CallbackMenu.AlarmHours] = SettingsHandler.HandleAlarmHours,
    };

    private static readonly Dictionary<CallbackMenu, BuildEventStatus> ReplaceStatusDict = new()
    {
        [CallbackMenu.EventTitleReplace] = BuildEventStatus.CreateTitle,
        [CallbackMenu.EventDateTimeOfAgain] = BuildEventStatus.CreateDateTimeOf,
        [CallbackMenu.EventDateTimeOfReplace] = BuildEventStatus.CreateDateTimeOf,
        [CallbackMenu.EventAddressReplace] = BuildEventStatus.CreateAddress,
        [CallbackMenu.EventCostReplace] = BuildEventStatus.CreateCost,
        [CallbackMenu.EventPartLimitReplace] = BuildEventStatus.CreateParticipantLimit,
    };

    private static readonly IEventService EService = new DbEventService();
    private static readonly ILog Log = LogManager.GetLogger(typeof(MenuHandler)); 

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Menu is not { } menu)
            throw new NullReferenceException("CallbackData doesn't have menu");
        MenuHandlerDict.GetValueOrDefault(menu, CallbackUnknownMenu).Invoke(container);
    }

    private static void HandleEventReplaceMenu(UpdateContainer container)
    {
        if (!ReplaceStatusDict.TryGetValue(container.CallbackData!.Menu!.Value, out BuildEventStatus status))
        {
            Log.Error("Unknown CallbackMenu");
        }

        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        if (container.CallbackData!.Button! == CallbackButton.Yes)
        {
            MessageSender.SendEnterDataRequest(container, status);
        }
    }

    private static void HandleEventRegisterMenu(UpdateContainer container)
    {
        IndividualEventHandler.HandleRegister(container);
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Log.Error("Pressing button in unknown menu.");
    }
}