using System.Globalization;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

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
                string text = $"Название: {appEvent.Title}\n" +
                              $"Организатор: @{appEvent.Creator.Username}\n" +
                              $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                              $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                              $"Место: {appEvent.Address}\n" +
                              $"Максимум человек: {appEvent.ParticipantLimit}\n" +
                              $"Зарегистрировалось: {appEvent.RegisteredUsers.Count}\n" +
                              $"Планируемые затраты: {appEvent.Cost}\n" +
                              (string.IsNullOrEmpty(appEvent.Description)
                                  ? ""
                                  : $"Дополнительная информация: {appEvent.Description}");
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: text,
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
                string text = $"Название: {appEvent.Title}\n" +
                              $"Организатор: @{appEvent.Creator.Username}\n" +
                              $"Дата: {appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                              $"Время: {appEvent.DateTimeOf!.Value:HH:mm}\n" +
                              $"Место: {appEvent.Address}\n" +
                              $"Максимум человек: {appEvent.ParticipantLimit}\n" +
                              $"Зарегистрировалось: {appEvent.RegisteredUsers.Count}\n" +
                              $"Планируемые затраты: {appEvent.Cost}\n" +
                              (string.IsNullOrEmpty(appEvent.Description)
                                  ? ""
                                  : $"Дополнительная информация: {appEvent.Description}");
                Thread.Sleep(200);
                container.BotClient.SendMessage(
                    chatId: container.ChatId,
                    text: text,
                    replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegisterToEvent, appEvent.Id));
            }
            Thread.Sleep(200);
            container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
            DataService.UpdateUserStatus(container, UserStatus.Active, UService);
        }
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