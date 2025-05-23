using System.Globalization;
using System.Text;
using Com.Github.PatBatTB.GEBB.DataBase.Event;
using Com.Github.PatBatTB.GEBB.DataBase.User;
using Com.Github.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Domain.Enums;
using Com.GitHub.PatBatTB.GEBB.Exceptions;
using Com.Github.PatBatTB.GEBB.Services.Providers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Com.Github.PatBatTB.GEBB.Services.Handlers.Types.Callback.Button;

public static class EventListHandler
{
    private static readonly IEventService EService = new DbEventService();
    private static readonly IUserService UService = new DbUserService();

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MyOwnButtonDict = new()
    {
        [CallbackButton.PartList] = HandleMyOwnParticipantList,
        [CallbackButton.Edit] = HandleEdit,
        [CallbackButton.Cancel] = HandleCancel,
        [CallbackButton.Close] = HandleClose,
    };

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> MyOwnButtonPartDict = new()
    {
        [CallbackButton.ToDescr] = HandleMyOwnToDescription,
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

    private static readonly Dictionary<CallbackButton, Action<UpdateContainer>> RegisterButtonDist = new()
    {
        [CallbackButton.Reg] = HandleRegisterToEvent,
        [CallbackButton.Close] = HandleClose,
    };

    public static void HandleMyOwn(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        MyOwnButtonDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    public static void HandleMyOwnPart(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        MyOwnButtonPartDict.GetValueOrDefault(button, HandleUnknown).Invoke(container);
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

    public static void HandleRegister(UpdateContainer container)
    {
        if (container.CallbackData?.Button is not { } button)
            throw new NullReferenceException("CallbackData doesn't have button");
        RegisterButtonDist.GetValueOrDefault(button, HandleUnknown).Invoke(container);
    }

    private static void HandleMyOwnParticipantList(UpdateContainer container)
    {
        AppEvent appEvent = EService.Get(container.CallbackData!.EventId!);
        string participantList = GetParticipantList(appEvent);
        string text = $"Название: {appEvent.Title}\n" +
                      $"Участники:\n" +
                      $"{participantList}";
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreEventPart, appEvent.Id),
            cancellationToken: container.Token);
    }

    private static void HandleMyOwnToDescription(UpdateContainer container)
    {
        AppEvent appEvent = EService.Get(container.CallbackData!.EventId!);
        string text = $"Название: {appEvent.Title}\n" +
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
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.CreatedEvent, appEvent.Id),  
            cancellationToken: container.Token);
    }

    private static void HandleEdit(UpdateContainer container)
    {
        try
        {
            container.Events.Add(EService.Get(container.CallbackData!.EventId!));
            List<AppEvent> appEvents = new();
            appEvents.AddRange(EService.GetBuildEvents(container.ChatId, EventStatus.Editing));
            if (appEvents.Count > 0) throw new InvalidOperationException();
            Thread.Sleep(200);
            Message sent = container.BotClient.SendMessage(
                chatId: container.ChatId,
                text: $"Редактирование мероприятия:\n{container.Events[0].Title}",
                cancellationToken: container.Token).Result;
            container.Events[0].MessageId = sent.Id;
            container.Events[0] = EService.Edit(container.Events[0]);
            Thread.Sleep(100);
            container.BotClient.EditMessageReplyMarkup(
                chatId: container.ChatId,
                messageId: sent.Id,
                replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.EditEvent, container.Events[0].Id),
                cancellationToken: container.Token);
            DataService.UpdateUserStatus(container, UserStatus.EditingEvent, UService);
        }
        catch (EntityNotFoundException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка, текущее мероприятие не найдено в базе.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        catch (InvalidOperationException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Произошла ошибка. В базе найдено другое мероприятие, которые вы редактируете. Попробуйте снова.",
                showAlert: true,
                cancellationToken: container.Token);
            ICollection<int> editingIds = EService.RemoveInBuilding(container.AppUser.UserId, EventStatus.Editing);
            container.BotClient.DeleteMessages(container.ChatId, editingIds, container.Token);
        }
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(container.ChatId, container.Message.Id, container.Token);
    }

    private static void HandleCancel(UpdateContainer container)
    {
        AppEvent appEvent = EService.Get(container.CallbackData!.EventId!);
        EService.Remove(container.CallbackData!.EventId!);
        container.BotClient.AnswerCallbackQuery(
            callbackQueryId: container.CallbackData.CallbackId!,
            text: "Мероприятие отменено.",
            showAlert: true,
            cancellationToken: container.Token);
        Thread.Sleep(200);
        string text = $"Мероприятие {appEvent.Title}\n" +
                      $"{appEvent.DateTimeOf!.Value.ToString("ddd dd MMMM yyyy", new CultureInfo("ru-RU"))}\n" +
                      $"{appEvent.DateTimeOf!.Value:HH:mm}\n" +
                      $"отменено организатором.";
        foreach (AppUser appUser in appEvent.RegisteredUsers)
        {
            Thread.Sleep(200);
            container.BotClient.SendMessage(
                chatId: appUser.UserId,
                text: text,
                cancellationToken: container.Token);
        }

        HandleClose(container);
    }

    private static void HandleParticipantList(UpdateContainer container)
    {
        AppEvent appEvent = EService.Get(container.CallbackData?.EventId!);
        string participantList = GetParticipantList(appEvent);
        
        string text = $"Название: {appEvent.Title}\n" +
                      $"Организатор: @{appEvent.Creator.Username}\n" +
                      $"Участники:\n" +
                      $"{participantList}";
        Thread.Sleep(200);
        container.BotClient.EditMessageText(
            container.ChatId,
            container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventPart, appEvent.Id),
            cancellationToken: container.Token);
    }

    private static void HandleCancelRegistration(UpdateContainer container)
    {
        AppEvent appEvent = EService.Get(container.CallbackData?.EventId!);
        EService.CancelRegistration(appEvent, container.AppUser);
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
        AppEvent appEvent = EService.Get(container.CallbackData?.EventId!);
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
        container.BotClient.EditMessageText(
            chatId: container.ChatId,
            messageId: container.Message.Id,
            text: text,
            replyMarkup: InlineKeyboardProvider.GetMarkup(CallbackMenu.RegEventDescr, appEvent.Id),
            cancellationToken: container.Token);
    }

    private static void HandleRegisterToEvent(UpdateContainer container)
    {
        try
        {
            AppEvent appEvent = EService.Get(container.CallbackData!.EventId!);
            
            if (appEvent.ParticipantLimit > 0 && appEvent.ParticipantLimit <= appEvent.RegisteredUsers.Count)
            {
                Thread.Sleep(200);
                container.BotClient.AnswerCallbackQuery(
                    callbackQueryId: container.CallbackData!.CallbackId!,
                    text: "К сожалению, места на это мероприятие закончились.",
                    showAlert: true,
                    cancellationToken: container.Token);
            }
            else
            {
                EService.RegisterUser(appEvent, container.AppUser);
                Thread.Sleep(200);
                container.BotClient.AnswerCallbackQuery(
                    callbackQueryId: container.CallbackData!.CallbackId!,
                    text: "Вы успешно зарегистрировались на мероприятие.",
                    showAlert: true,
                    cancellationToken: container.Token);
            }
        }
        catch (EntityNotFoundException)
        {
            Thread.Sleep(200);
            container.BotClient.AnswerCallbackQuery(
                callbackQueryId: container.CallbackData!.CallbackId!,
                text: "Данное мероприятие больше неактуально.",
                showAlert: true,
                cancellationToken: container.Token);
        }
        
        Thread.Sleep(200);
        container.BotClient.DeleteMessage(
            chatId: container.ChatId,
            messageId: container.Message.Id,
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

    private static string GetParticipantList(AppEvent appEvent)
    {
        StringBuilder participantList = new();
        foreach (AppUser appUser in appEvent.RegisteredUsers)
        {
            participantList.Append("@" + appUser.Username + "\n");
        }

        return participantList.ToString();
    }

    private static void HandleUnknown(UpdateContainer container)
    {
        Console.WriteLine("EventListHandler.UnknownButton");
    }
}