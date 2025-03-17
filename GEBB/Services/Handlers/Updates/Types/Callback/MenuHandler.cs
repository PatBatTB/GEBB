using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback;

public static class MenuHandler
{
    private static readonly Dictionary<CallbackMenu, Action<UpdateContainer>> MenuHandlerDict = new()
    {
        [CallbackMenu.Main] = MainHandler.Handle,
        [CallbackMenu.MyEvents] = MyEventsHandler.Handle,
        [CallbackMenu.CreateEvent] = CreateEventHandler.Handle,
        [CallbackMenu.EventTitleReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfAgain] = HandleEventReplaceMenu,
        [CallbackMenu.EventDateTimeOfReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventAddressReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventCostReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventParticipantLimitReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventDescriptionReplace] = HandleEventReplaceMenu,
        [CallbackMenu.EventRegister] = EventRegisterMenuHandle,
    };

    private static readonly Dictionary<CallbackMenu, CreateEventStatus> ReplaceStatusDict = new()
    {
        [CallbackMenu.EventTitleReplace] = CreateEventStatus.Title,
        [CallbackMenu.EventDateTimeOfAgain] = CreateEventStatus.DateTimeOf,
        [CallbackMenu.EventDateTimeOfReplace] = CreateEventStatus.DateTimeOf,
        [CallbackMenu.EventAddressReplace] = CreateEventStatus.Address,
        [CallbackMenu.EventCostReplace] = CreateEventStatus.Cost,
        [CallbackMenu.EventParticipantLimitReplace] = CreateEventStatus.ParticipantLimit,
    };

    public static void Handle(UpdateContainer container)
    {
        if (container.AlterCbData?.Menu is not { } menu)
            throw new NullReferenceException("CallbackData doesn't have menu");
        MenuHandlerDict.GetValueOrDefault(menu, CallbackUnknownMenu)
            .Invoke(container);
    }

    private static void HandleEventReplaceMenu(UpdateContainer container)
    {
        //TODO add unknown
        if (!ReplaceStatusDict.TryGetValue(container.AlterCbData!.Menu!.Value, out CreateEventStatus status))
        {
            throw new ArgumentException("Unknown CallbackMenu");
        }

        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        if (container.AlterCbData!.Button! == CallbackButton.Yes)
        {
            MessageSender.SendEnterDataRequest(container, status);
        }
    }

    private static void EventRegisterMenuHandle(UpdateContainer container)
    {
        //TODO распарсить мероприятие.
        //TODO проверить, есть ли места
        //TODO если есть - зарегистрировать.
        //TODO если нет - сообщение, что места закончились.
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CallbackMenuUnknown()");
    }
}