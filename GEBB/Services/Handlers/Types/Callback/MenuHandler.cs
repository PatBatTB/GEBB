using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback;

public static class MenuHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> MenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainHandler.Handle,
        [CallbackMenu.MyEvents] = MyEventsHandler.Handle,
        [CallbackMenu.CreateEvent] = BuildEventHandler.Handle,
        [CallbackMenu.EditEvent] = BuildEventHandler.Handle,
        [CallbackMenu.EventTitleReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfAgain] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventAddressReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventCostReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventPartLimitReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDescrReplace] = HandleEventReplaceMenu,
        [CallbackMenu.RegisterToEvent] = HandleEventRegisterMenu,
        [CallbackMenu.CreatedEvent] = EventListHandler.HandleMyOwn,
        [CallbackMenu.CreEventPart] = EventListHandler.HandleMyOwnPart,
        [CallbackMenu.RegEventDescr] = EventListHandler.HandleRegisteredDescr,
        [CallbackMenu.RegEventPart] = EventListHandler.HandleRegisteredPart,
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
            throw new ArgumentException("Unknown CallbackMenu");
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
        EventListHandler.HandleRegister(container);
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CallbackMenuUnknown()");
    }
}