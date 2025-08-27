using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback;

public class MenuHandler
{
    private readonly Dictionary<CallbackMenu, Action<UpdateContainer>> _menuHandlerDict;
    private readonly Dictionary<CallbackMenu, BuildEventStatus> _replaceStatusDict;
    private readonly IndividualEventHandler _individualEventHandler;
    private readonly ILog _log;

    public MenuHandler()
    {
        _individualEventHandler = new IndividualEventHandler();
        MainHandler mainHandler = new();
        BuildEventHandler buildEventHandler = new();
        EventsListHandler eventsListHandler = new();
        SettingsHandler settingsHandler = new();
        
        _menuHandlerDict = new Dictionary<CallbackMenu, Action<UpdateContainer>>
        {
            [CallbackMenu.Main] = mainHandler.Handle,
            [CallbackMenu.CreateEvent] = buildEventHandler.Handle,
            [CallbackMenu.EventsList] = eventsListHandler.Handle,
            [CallbackMenu.EditEvent] = buildEventHandler.Handle,
            [CallbackMenu.EventTitleReplace] = HandleEventReplaceMenu,
            [CallbackMenu.EventDateTimeOfAgain] = HandleEventReplaceMenu,
            [CallbackMenu.EventDateTimeOfReplace] = HandleEventReplaceMenu,
            [CallbackMenu.EventAddressReplace] = HandleEventReplaceMenu,
            [CallbackMenu.EventCostReplace] = HandleEventReplaceMenu,
            [CallbackMenu.EventPartLimitReplace] = HandleEventReplaceMenu,
            [CallbackMenu.EventDescrReplace] = HandleEventReplaceMenu,
            [CallbackMenu.RegisterToEvent] = HandleEventRegisterMenu,
            [CallbackMenu.CreatedEvent] = _individualEventHandler.HandleMyOwn,
            [CallbackMenu.CreEventPart] = _individualEventHandler.HandleMyOwnPart,
            [CallbackMenu.RegEventDescr] = _individualEventHandler.HandleRegisteredDescr,
            [CallbackMenu.RegEventPart] = _individualEventHandler.HandleRegisteredPart,
            [CallbackMenu.Settings] = settingsHandler.Handle,
            [CallbackMenu.Alarm] = settingsHandler.HandleAlarmMenu,
            [CallbackMenu.AlarmHours] = settingsHandler.HandleAlarmHours,
        };
        
        _replaceStatusDict = new Dictionary<CallbackMenu, BuildEventStatus>
        {
            [CallbackMenu.EventTitleReplace] = BuildEventStatus.CreateTitle,
            [CallbackMenu.EventDateTimeOfAgain] = BuildEventStatus.CreateDateTimeOf,
            [CallbackMenu.EventDateTimeOfReplace] = BuildEventStatus.CreateDateTimeOf,
            [CallbackMenu.EventAddressReplace] = BuildEventStatus.CreateAddress,
            [CallbackMenu.EventCostReplace] = BuildEventStatus.CreateCost,
            [CallbackMenu.EventPartLimitReplace] = BuildEventStatus.CreateParticipantLimit,
        };
        
        _log = LogManager.GetLogger(typeof(MenuHandler));
    }

    public void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Menu is not { } menu)
            throw new NullReferenceException("CallbackData doesn't have menu");
        _menuHandlerDict.GetValueOrDefault(menu, CallbackUnknownMenu).Invoke(container);
    }

    private void HandleEventReplaceMenu(UpdateContainer container)
    {
        if (!_replaceStatusDict.TryGetValue(container.CallbackData!.Menu!.Value, out BuildEventStatus status))
        {
            _log.Error("Unknown CallbackMenu");
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

    private void HandleEventRegisterMenu(UpdateContainer container)
    {
        _individualEventHandler.HandleRegister(container);
    }

    private void CallbackUnknownMenu(UpdateContainer container)
    {
        _log.Error("Pressing button in unknown menu.");
    }
}