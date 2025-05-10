using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class MainHandler
{
    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> ButtonHandlerDict = new()
    {
        [CallbackButton.MyEvents] = HandleEvents,
        [CallbackButton.MyRegs] = HandleMyRegistrations,
        [CallbackButton.AvailEvents] = HandleAvailableEvents,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly IUserService UService = new DbUserService();
    private static readonly IEventService EService = new DbEventService();

    public static void Handle(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        ButtonHandlerDict.GetValueOrDefault(button, HandleUnknown)
            .Invoke(container);
    }

    private static void HandleEvents(UpdateContainer container)
    {
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            CallbackMenu.MyEvents.Text(),
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.MyEvents),
            cancellationToken: container.Token);
    }

    private static void HandleMyRegistrations(UpdateContainer container)
    {
        List<EventDto> eventList = container.Events;
        eventList.AddRange(EService.GetRegisterEvents(container.UserDto.UserId));
        string noEventsText = "Вы не зарегистрированы ни на одно мероприятие.";
        if (eventList.Count == 0)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: noEventsText,
                showAlert: true,
                cancellationToken: container.Token);
        }
        else
        {
            foreach (EventDto eventDto in container.Events)
            {
                string text = $"Название: {eventDto.Title}\n" +
                              $"Организатор: {eventDto.Creator.Username}\n" +
                              $"Дата: {eventDto.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                              $"Время: {eventDto.DateTimeOf!.Value:HH:mm}\n" +
                              $"Место: {eventDto.Address}\n" +
                              $"Максимум человек: {eventDto.ParticipantLimit}\n" +
                              $"Зарегистрировалось: {eventDto.RegisteredUsers.Count}\n" +
                              $"Планируемые затраты: {eventDto.Cost}\n" +
                              (string.IsNullOrEmpty(eventDto.Description)
                                  ? ""
                                  : $"Дополнительная информация: {eventDto.Description}");
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: text,
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventDescr, eventDto.EventId),
                    cancellationToken: container.Token);
            }
        }
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }

    private static void HandleAvailableEvents(UpdateContainer container)
    {
        //TODO получить список мероприятий с датой в будущем, со свободными местами и куда еще не зарегистрирован.
        //TODO отправить пользователю списком сообщений (добавить кнопки регистрации и закрыть).
        throw new NotImplementedException();
    }

    private static void HandleClose(UpdateContainer container)
    {
        var chatId = container.ChatId;
        var messageId = container.Message.Id;
        container.BotClient.DeleteMessage(
            chatId,
            messageId,
            container.Token);
        DataService.UpdateUserStatus(container, UserStatus.Active, UService);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("MenuButtonHandler.MainMenuUnknownButtonHandle()");
    }
}