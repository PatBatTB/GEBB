using Com.GitHub.PatBatTB.GEBB;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using log4net;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class EventsListHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonDict = new()
    {
        [CallbackButton.MyEvents] = HandleMyEvents,
        [CallbackButton.MyRegs] = HandleMyRegistrations,
        [CallbackButton.AvailEvents] = HandleAvailableEvents,
        [CallbackButton.Back] = HandleBack,
    };

    private static readonly ILog Log = LogManager.GetLogger(typeof(EventsListHandler));
    private static readonly IEventService EService = App.ServiceFactory.GetEventService();
    private static readonly IUserService UService = App.ServiceFactory.GetUserService();

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData!.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }
    
    private static void HandleMyEvents(UpdateContainer container)
    {
        container.Events.AddRange(EService.GetMyOwnEvents(container.AppUser.UserId));
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
    
    private static void HandleMyRegistrations(UpdateContainer container)
    {
        List<AppEvent> eventList = container.Events;
        eventList.AddRange(EService.GetRegisterEvents(container.AppUser.UserId));
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
            DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        }
    }
    
    private static void HandleAvailableEvents(UpdateContainer container)
    {
        container.Events.AddRange(EService.GetAvailableEvents(container.AppUser.UserId));
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
            DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        }
    }

    private static void HandleBack(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: CallbackMenu.Main.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.Main),
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Log.Error("Unknown button");
    }
}