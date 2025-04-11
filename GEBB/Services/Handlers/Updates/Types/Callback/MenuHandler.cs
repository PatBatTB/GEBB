using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

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
        [CallbackMenu.EventHandle] = EventListHandler.Handle,
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

    private static readonly IEventService EService = new DbEventService(); 

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Menu is not { } menu)
            throw new NullReferenceException("CallbackData doesn't have menu");
        MenuHandlerDict.GetValueOrDefault(menu, CallbackUnknownMenu)
            .Invoke(container);
    }

    private static void HandleEventReplaceMenu(UpdateContainer container)
    {
        if (!ReplaceStatusDict.TryGetValue(container.CallbackData!.Menu!.Value, out CreateEventStatus status))
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

    private static void EventRegisterMenuHandle(UpdateContainer container)
    {
        if (EService.Get(container.CallbackData?.EventId!) is not { } eventDto)
        {
            throw new Exception("Event not found in DB");
        }
        
        if (eventDto.ParticipantLimit > 0 && eventDto.ParticipantLimit <= eventDto.RegisteredUsers.Count)
        {
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "К сожалению, места на это мероприятие закончились.",
                showAlert: true,
                cancellationToken: container.Token);
            Thread.Sleep(200);
        }
        else
        {
            EService.RegisterUser(eventDto, container.UserDto);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Вы успешно зарегистрировались на мероприятие.",
                showAlert: true,
                cancellationToken: container.Token);
            Thread.Sleep(200);
        }
        
        container.BotClient.EditMessageReplyMarkup(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            replyMarkup: InlineKeyboardMarkup.Empty(),
            cancellationToken: container.Token);
        Thread.Sleep(200);
    }

    private static void CallbackUnknownMenu(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.CallbackMenuUnknown()");
    }
}