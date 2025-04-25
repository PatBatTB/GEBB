using System.Globalization;
using System.Text;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Updates.Types.Callback.Menu;

public static class EventListHandler
{
    private static readonly IEventService EService = new DbEventService();

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MyOwnButtonDict = new()
    {
        [CallbackButton.Edit] = HandleEdit,
        [CallbackButton.Cancel] = HandleCancel,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> RegisteredButtonDescrDict = new()
    {
        [CallbackButton.PartList] = HandleParticipantList,
        [CallbackButton.CancelReg] = HandleCancelRegistration,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> RegisteredButtonPartDict = new()
    {
        [CallbackButton.ToDescr] = HandleToDescription,
        [CallbackButton.CancelReg] = HandleCancelRegistration,
        [CallbackButton.Close] = HandleClose,
    };

    public static void HandleMyOwn(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        MyOwnButtonDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public static void HandleRegisteredDescr(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        RegisteredButtonDescrDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public static void HandleRegisteredPart(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        RegisteredButtonPartDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private static void HandleEdit(UpdateContainer container)
    {
        //TODO открыть форму редактирования мероприятия (аналогично созданию)
    }

    private static void HandleCancel(UpdateContainer container)
    {
        //TODO Всплывающее сообщение организатору, что мероприятие отменено.
        EventDto eventDto = EService.Get(container.CallbackData!.EventId!)!;
        EService.Remove(container.CallbackData!.EventId!);
        string text = $"Мероприятие {eventDto.Title}\n" +
                      $"{eventDto.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"{eventDto.DateTimeOf!.Value:HH:mm}\n" +
                      $"отменено организатором.";
        foreach (UserDto userDto in eventDto.RegisteredUsers)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: userDto.UserId,
                text: text,
                cancellationToken: container.Token);
        }

        HandleClose(container);
    }

    private static void HandleParticipantList(UpdateContainer container)
    {
        if (EService.Get(container.CallbackData?.EventId!) is not { } eventDto)
            throw new NullReferenceException("Event not found in db");

        StringBuilder participantList = new();
        foreach (UserDto userDto in eventDto.RegisteredUsers)
        {
            participantList.Append("@" + userDto.Username + "\n");
        }
        string text = $"Название: {eventDto.Title}\n" +
                      $"Организатор: @{eventDto.Creator.Username}\n" +
                      $"Участники:\n" +
                      $"{participantList}";
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventPart, eventDto.EventId),
            cancellationToken: container.Token);
    }

    private static void HandleCancelRegistration(UpdateContainer container)
    {
        
        //TODO Пытается добавить новые записи, вместо того, что бы удалить.
        if (EService.Get(container.CallbackData?.EventId!) is not { } eventDto)
            throw new NullReferenceException("Event not found in db");
        EService.CancelRegistration(eventDto, container.UserDto);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        container.BotClient.AnswerCallbackQuery(
            callbackQueryId: container.CallbackData!.CallbackId!,
            text: "Вы отменили регистрацию на мероприятие.",
            showAlert: true,
            cancellationToken: container.Token);
    }

    private static void HandleToDescription(UpdateContainer container)
    {
        if (EService.Get(container.CallbackData?.EventId!) is not { } eventDto)
            throw new NullReferenceException("Event not found in db");
        string text = $"Название: {eventDto.Title}\n" +
                      $"Организатор: @{eventDto.Creator.Username}\n" +
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
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventDescr, eventDto.EventId),
            cancellationToken: container.Token);
    }

    private static void HandleClose(UpdateContainer container)
    {
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            cancellationToken: container.Token);
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("EventListHandler.UnknownButton");
    }
}