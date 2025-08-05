using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public class EventsListHandler
{
    private readonly Dictionary<CallbackButton, Action<UpdateContainer>> _buttonDict;

    private readonly IEventService _eService;
    private readonly IUserService _uService;
    
    private readonly ILog _log;

    public EventsListHandler()
    {
        _uService = App.ServiceFactory.GetUserService();
        _eService = App.ServiceFactory.GetEventService();
        _log = LogManager.GetLogger(typeof(EventsListHandler));
        _buttonDict = new Dictionary<CallbackButton, Action<UpdateContainer>>
        {
            [CallbackButton.MyEvents] = HandleMyEvents,
            [CallbackButton.MyRegs] = HandleMyRegistrations,
            [CallbackButton.AvailEvents] = HandleAvailableEvents,
            [CallbackButton.Back] = HandleBack,
        };
    }

    public void Handle(UpdateContainer container)
    {
        if (container.CallbackData!.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        _buttonDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }
    
    private void HandleMyEvents(UpdateContainer container)
    {
        container.Events.AddRange(_eService.GetMyOwnEvents(container.AppUser.UserId));
        if (container.Events.Count > 0)
        {
            foreach (AppEvent appEvent in container.Events)
            {
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: MessageService.GetMyEventDescription(appEvent),
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreatedEvent, appEvent.Id),  
                    cancellationToken: container.Token);
            }
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(
                chatId: container.ChatId,
                messageId: container.Message.Id,
                cancellationToken: container.Token);
        }
        else
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Созданных мероприятий нет.",
                showAlert: true,
                cancellationToken: container.Token);
        }
    }
    
    private void HandleMyRegistrations(UpdateContainer container)
    {
        List<AppEvent> eventList = container.Events;
        eventList.AddRange(_eService.GetRegisterEvents(container.AppUser.UserId));
        if (eventList.Count == 0)
        {
            string noEventsText = "Вы не зарегистрированы ни на одно мероприятие.";
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: noEventsText,
                showAlert: true,
                cancellationToken: container.Token);
        }
        else
        {
            foreach (AppEvent appEvent in container.Events)
            {
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: MessageService.GetEventDescription(appEvent),
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventDescr, appEvent.Id),
                    cancellationToken: container.Token);
            }
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
            DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        }
    }
    
    private void HandleAvailableEvents(UpdateContainer container)
    {
        container.Events.AddRange(_eService.GetAvailableEvents(container.AppUser.UserId));
        if (container.Events.Count == 0)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "В настоящее время нет доступных для регистрации мероприятий.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        else
        {
            foreach (AppEvent appEvent in container.Events)
            {
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: MessageService.GetEventDescription(appEvent),
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegisterToEvent, appEvent.Id));
            }
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
            DataService.UpdateUserStatus(container, UserStatus.Active, _uService);
        }
    }

    private void HandleBack(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private void HandleUnknown(UpdateContainer container)
    {
        _log.Error("Unknown button");
    }
}